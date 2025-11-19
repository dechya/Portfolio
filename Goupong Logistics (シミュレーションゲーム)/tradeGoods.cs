using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using static goodsDB;
using AssetKits.ParticleImage;
using Unity.VisualScripting;

[System.Serializable]
public struct SavedStocks
{
    public int stock;
    public int price;
}

public class tradeGoods : MonoBehaviour
{
    public static tradeGoods Instance;
    public SoundManager soundManager;
    public goodsDB goodsDB;

    //계산용 변수
    private int totalCost;
    private int purCost;
    private int saleCost;

    public UnityEvent onCargoWindowEnabled;

    private string selectedProvince;

    [Header("배송 지역 특산물")]
    [SerializeField] private GameObject preview;

    [Header("각 창에 생성된 프리팹 리스트")]
    public List<GameObject> instantiatedPrefabs = new List<GameObject>();
    public List<GameObject> newgoods = new List<GameObject>();
    public List<GameObject> inventories = new List<GameObject>();

    [Header("처음 재고 저장 할 배열")]
    public SavedStocks[] Sstock = new SavedStocks[90];

    [Header("지역화물")]
    [SerializeField] private GameObject TradeChang; // 지역화물 프리팹이 생성될 창
    [SerializeField] private GameObject LocalTradePrefab; // 지역화물 프리팹

    [Header("거래화물")]
    [SerializeField] private GameObject PurchaseChang; // 거래화물 프리팹이 생성될 창
    [SerializeField] private GameObject purchasePrefab; // 거래화물 프리팹
    public GameObject salePrefab; // 거래창에 생성되는 판매 프리팹
    public GameObject costSum; // 구매, 판매가격 합산    

    [Header("소유화물")]
    public GameObject inventransfrom; // 소유화물 프리팹이 생성될 창
    public GameObject invenPrefab; // 소유화물 프리팹

    [Header("적재량 슬라이더")]
    public Slider capacitySlider;
    [HideInInspector] public float haveCapacity;
    [HideInInspector] public bool disCount = false;            
    [HideInInspector] public bool halfSpeedApplied = false;                 

    [Header("무게 텍스트")]
    public Text weightText;

    [Header("초기화용 불")]
    public bool exit = false;
    [HideInInspector] public bool isUp = false;

    public ParticleImage particleImage;

    int previousMonth;// 이전달을 저장할 변수
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

    }
    public void Start()
    {
        particleImage.duration = 0.1f;
        initStock();
        previousMonth = GameManager.Instance.curMonth;
    }
    
    public void OnEnable()
    {
        //StockInitializer();
        UpdateInventoryPrices();
        DisplaylocalPrefab(selectedProvince);
        UpdateLocalPrefabText();
        SetPlayerProperties();        
    }

    private void Update()
    {
        SetPlayerProperties();
    }

    public void SetPlayerProperties()
    {
        capacitySlider.minValue = 0;
        capacitySlider.maxValue = GameManager.Instance.playerProperties.currentPlayerCapacity;
        weightText.text = (haveCapacity).ToString("N0") + " / " + GameManager.Instance.playerProperties.currentPlayerCapacity.ToString("N0"); // 무게 텍스트 업데이트       
    }

    // 생성된 프리팹에 버튼 추가
    public void addButtonToLocalPrefab()
    {
        foreach (GameObject instantiatedPrefab in instantiatedPrefabs)
        {
            Button prefabButton = instantiatedPrefab.GetComponent<Button>();
            prefabButton.onClick.RemoveAllListeners();
            prefabButton.onClick.AddListener(OnPrefabButtonClick);
        }
    }

    public void addButtonToTradePrefab()
    {
        foreach(GameObject purchasePrefab in newgoods)
        {
            Button prefabButton = purchasePrefab.GetComponent<Button>();
            prefabButton.onClick.RemoveAllListeners();
            prefabButton.onClick.AddListener(ReturnGoodsToLocal);
        }
    }

    // 지역화물에 특산물 추가
    private void DisplaylocalPrefab(string prvName)
    {        
        //foreach (GameObject prefab in instantiatedPrefabs)
        //    instantiatedPrefabs.Remove(prefab);

        //instantiatedPrefabs.Clear();

        for (int i = 0; i < goodsDB.Local.Count; i++)
        {

            if (prvName == goodsDB.Local[i].도 && goodsDB.Local[i].재고 > 0)
            {
                GameObject trade = Instantiate(LocalTradePrefab, TradeChang.transform);
                Debug.Log("tarde : " + trade.gameObject.name);
                instantiatedPrefabs.Add(trade);

                Transform prefabTransform = trade.transform;

                Text goodsNameText = prefabTransform.GetChild(0).GetComponent<Text>();
                goodsNameText.text = goodsDB.Local[i].특산물;

                Text stockText = prefabTransform.GetChild(2).GetComponent<Text>();
                stockText.text = "재고: " + goodsDB.Local[i].재고.ToString();

                Text priceText = prefabTransform.GetChild(3).GetComponent<Text>();
                priceText.text = goodsDB.Local[i].현지가격.ToString();
            }
        }
        // Add button functionality
        addButtonToLocalPrefab();
    }

    public void scriptableObject()
    {
        selectedProvince = EventSystem.current.currentSelectedGameObject.name;
        // DisplaylocalPrefab(selectedProvince);
    }

    // for문이나 if문이나 코루틴써서 가격 한번에 관리
    // 지역화물 버튼 클릭시    
    public void OnPrefabButtonClick()
    {
        soundManager.PlaySFX("메뉴클릭");
        if ((haveCapacity) < capacitySlider.maxValue)
        {
            Button clickedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();//클릭된 버튼의 정보를 가져오기
            Text buttonText = clickedButton.GetComponentInChildren<Text>();
            Text stockText = clickedButton.transform.GetChild(2).GetComponent<Text>();

            string selectedGoodsName = buttonText.text; // 클릭된 버튼의 텍스트를 가져오기

            int dbindex = goodsDB.Local.FindIndex(g => g.특산물 == selectedGoodsName);
            float weight = (goodsDB.Local[dbindex].무게 * 1000); //클릭된 버튼의 특산물이름을 goodsDB에서 찾아서 인덱스값을 가져오기
            IncreaseWeight(weight);
            goodsDB.Local[dbindex].재고 -= 1;//클릭된 특산물의 재고를 1감소시키기

            stockText.text = "재고:" + goodsDB.Local[dbindex].재고.ToString();//특산물의 재고를 텍스트로 표시

            if (goodsDB.Local[dbindex].재고 <= 0)
                clickedButton.gameObject.SetActive(false);
            
            displayPurchasePrefabText(buttonText.text);//구매할 프리팹목록에 텍스트 추가

        }
        else
            return;
        
        addButtonToTradePrefab();
    }
 

    // 소유화물 특산물 추가
    public void addInventory(string selectedGoods, int stock, int price)
    {
        // 중복 체크
        GameObject existingInventory = inventories.Find(inv => inv.transform.GetChild(0).GetComponent<Text>().text == selectedGoods);

        if (existingInventory != null)
        {
            // 이미 있는 특산물의 재고 업데이트
            Text stockText = existingInventory.transform.GetChild(2).GetComponent<Text>();
            int currentStock = int.Parse(stockText.text.Split(':')[1]);

            currentStock += stock;
            stockText.text = "재고: " + currentStock.ToString();
            existingInventory.SetActive(true);
            return; // 중복된 특산물이므로 함수 종료
        }

        // 중복되지 않는 특산물의 경우 새로운 인벤토리 아이템 생성
        GameObject inventoryItem = Instantiate(invenPrefab, inventransfrom.transform);

        Transform invenTransform = inventoryItem.transform;
        Transform ptransform = purchasePrefab.transform;

        Text gnameText = invenTransform.GetChild(0).GetComponent<Text>();
        gnameText.text = selectedGoods;

        Text stocktext = invenTransform.GetChild(2).GetComponent<Text>();
        stocktext.text = "재고: " + stock.ToString();

        Text costText = invenTransform.GetChild(3).GetComponent<Text>();
        costText.text = price.ToString();

        // 가져온 텍스트를 costtext에 설정

        // 구매 시점의 특산물 가격을 찾아서 인벤토리 아이템에 설정
        Button saleButton = inventoryItem.GetComponent<Button>();        
        saleButton.onClick.AddListener(() => OnSaleButtonClick(inventoryItem, gnameText.text));
        inventories.Add(inventoryItem);        
    }

    // 소유화물 버튼 클릭시
    public void OnSaleButtonClick(GameObject saleObject, string selectedGoodsName)
    {
        soundManager.PlaySFX("메뉴클릭");

        Button clickedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        GameObject existingGoods = newgoods.Find(g => g.transform.GetChild(0).GetComponent<Text>().text == selectedGoodsName && g.CompareTag("SalePrefab"));
        Text invnenstockText = clickedButton.transform.GetChild(2).GetComponent<Text>();

        int dbindex = goodsDB.Local.FindIndex(g => g.특산물 == selectedGoodsName);
        float weight = (goodsDB.Local[dbindex].무게 * 1000); //클릭된 버튼의 특산물이름을 goodsDB에서 찾아서 인덱스값을 가져오기
        DecreaseWeight(weight);

        // 인벤토리 아이템 재고 업데이트
        Text InvnenstockText = saleObject.transform.GetChild(2).GetComponent<Text>();
        int Invenstock = int.Parse(InvnenstockText.text.Split(':')[1]);

        if (Invenstock > 0)
        {            
            Invenstock -= 1;
            InvnenstockText.text = "재고:" + Invenstock.ToString(); // 인벤토리의 특산물 재고 감소

        }
        if (Invenstock == 0)
        {
            saleObject.SetActive(false);
            //Destroy(saleObject); // 재고가 0이면 인벤토리 아이템 삭제
            //inventories.Remove(saleObject);
        }

        //판매 상품의 재고만 업데이트하고 함수 종료
        if (existingGoods != null)
        {
            Text stockText = existingGoods.transform.GetChild(2).GetComponent<Text>();
            int currentStock = int.Parse(stockText.text.Split(':')[1]);
            currentStock += 1;
            stockText.text = "재고:" + currentStock.ToString();

            // 총 가격 업데이트
            sumOfPurchaseAndSales();

            return;
        }

        // 새로운 판매 상품 생성
        saleObject = Instantiate(salePrefab, PurchaseChang.transform);

        Transform gnameTransform = saleObject.transform;
        Transform costTransform = saleObject.transform;
        Transform stockTransform = saleObject.transform;

        Text salegnameText = gnameTransform.GetChild(0).GetComponent<Text>();
        salegnameText.text = selectedGoodsName;

        Text salecostText = costTransform.GetChild(3).GetComponent<Text>();
        salecostText.text = clickedButton.transform.GetChild(3).GetComponent<Text>().text;

        Text stockTextNew = stockTransform.GetChild(2).GetComponent<Text>();
        int newStock = 1;
        stockTextNew.text = "재고:" + newStock.ToString();

        Button saleButton = saleObject.GetComponent<Button>();
        saleButton.onClick.RemoveAllListeners();
        saleButton.onClick.AddListener(ReturnGoodsToLocal);        
        newgoods.Add(saleObject);
        sumOfPurchaseAndSales();        
    }

    //희귀도별로 가격 업데이트
    private Dictionary<string, bool> isPriceAdjusted = new Dictionary<string, bool>();

    public void UpdateInventoryPrices()
    {
        float CalculateAdjustedPrice(float basePrice, string rarity, float originalPrice, string goodsName)
        {
            float multiplier = GetMultiplierByRarity(rarity);

            if (!isPriceAdjusted.ContainsKey(goodsName))
                isPriceAdjusted[goodsName] = false;

            if (!isPriceAdjusted[goodsName] && basePrice <= 2 * originalPrice)
            {
                isPriceAdjusted[goodsName] = true;
                return basePrice * multiplier;
            }
            else
                return basePrice;
        }
        for (int i = 0; i < inventories.Count; i++)
        {
            string goodsName = inventories[i].transform.GetChild(0).GetComponent<Text>().text;
            string rarity = GetRarityForGoods(goodsName);
            float originalPrice = float.Parse(inventories[i].transform.GetChild(3).GetComponent<Text>().text);
            float adjustedPrice = CalculateAdjustedPrice(originalPrice, rarity, originalPrice, goodsName);

            Text costText = inventories[i].transform.GetChild(3).GetComponent<Text>();
            costText.text = adjustedPrice.ToString();

            GameObject prefabWithGoodsName = instantiatedPrefabs.Find(prefab => prefab.transform.GetChild(0).GetComponent<Text>().text == goodsName);

            if (prefabWithGoodsName != null)
            {
                Text localPriceText = prefabWithGoodsName.transform.GetChild(3).GetComponent<Text>();
                localPriceText.text = adjustedPrice.ToString();
            }
        }
    }


    private string GetRarityForGoods(string goodsName)
    {
        var goods = goodsDB.Local.Find(item => item.특산물 == goodsName);
        if (goods != null)
            return goods.특산물등급; // 특산물의 등급을 반환
        else
        {
            // 특산물을 찾을 수 없는 경우 기본값을 반환
            return "일반";
        }
    }
    public float GetMultiplierByRarity(string rarityLevel)
    {
        switch (rarityLevel)
        {
            case "일반":
                isUp = true;
                return 1.2f;

            case "희귀":
                isUp = true;
                return 1.4f;

            case "고급":
                isUp = true;
                return 1.6f;

            default:
                break;
        }
        return 1.0f;
    }
    public void displayPurchasePrefabText(string selectedGoodsName)// 구매할 프리팹목록에 텍스트 추가
    {
        GameObject ClickedObject = EventSystem.current.currentSelectedGameObject;
        int dbIndex = goodsDB.Local.FindIndex(g => g.특산물 == selectedGoodsName);

        GameObject existingGoods = newgoods.Find(g => g.transform.GetChild(0).GetComponent<Text>().text == selectedGoodsName && g.CompareTag("LocalPrefab"));

        if (existingGoods != null)
        {
            Text stockText = existingGoods.transform.GetChild(2).GetComponent<Text>();
            int currentStock = int.Parse(stockText.text.Split(':')[1]);
            currentStock++;
            stockText.text = "재고:" + currentStock.ToString();

            // 총 가격 업데이트
            sumOfPurchaseAndSales();

            return; // 중복된 상품이므로 함수 종료
        }
        GameObject newGoods = Instantiate(purchasePrefab, PurchaseChang.transform);

        Transform gnameTransform = newGoods.transform;
        Transform costTransform = newGoods.transform;
        Transform stockTransform = newGoods.transform;

        Text gnameText = gnameTransform.GetChild(0).GetComponent<Text>();
        gnameText.text = selectedGoodsName;

        Text costText = costTransform.GetChild(3).GetComponent<Text>();
        costText.text = ClickedObject.transform.GetChild(3).GetComponent<Text>().text;

        Text stockTextNew = stockTransform.GetChild(2).GetComponent<Text>();
        int newStock = 1;
        stockTextNew.text = "재고:" + newStock.ToString();

        newgoods.Add(newGoods);

        // 총 가격 업데이트
        sumOfPurchaseAndSales();
    }

    // 거래창 구매, 판매가격 합산
    public void sumOfPurchaseAndSales() //구매,판매가격합산
    {
        purCost = 0;
        saleCost = 0;

        foreach (GameObject ngoods in newgoods)
        {
            if (ngoods.CompareTag("LocalPrefab"))
            {

                Transform pcostTransform = ngoods.transform;
                Text purText = pcostTransform.GetChild(3).GetComponent<Text>();
                int pcost = int.Parse(purText.text);
                int pstock = int.Parse(ngoods.transform.GetChild(2).GetComponent<Text>().text.Split(':')[1]);
                purCost += pcost * pstock;
            }
            else if (ngoods.CompareTag("SalePrefab"))
            {
                Transform scostTransform = ngoods.transform;
                Text costText = scostTransform.GetChild(3).GetComponent<Text>();
                int scost = int.Parse(costText.text);
                int sstock = int.Parse(ngoods.transform.GetChild(2).GetComponent<Text>().text.Split(':')[1]);
                saleCost += scost * sstock;
            }
            costSum.transform.GetChild(1).GetComponent<Text>().text = (saleCost - purCost).ToString();
            totalCost = int.Parse(costSum.transform.GetChild(1).GetComponent<Text>().text);

            if (totalCost <= 0)
                particleImage.duration = 0f;
            else if (totalCost > 0 && totalCost <= 100000f)
                particleImage.duration = 0.1f;
            else if (totalCost > 100000f && totalCost <= 1000000f)
                particleImage.duration = totalCost / 1000000f;
            else
                particleImage.duration = 1.0f;
        }
    }


    private void IncreaseWeight(float weight)
    {
        haveCapacity += weight;
        capacitySlider.value = haveCapacity; // 슬라이더 바에 무게 반영
        weightText.text = (haveCapacity).ToString("N0") + " / " + GameManager.Instance.playerProperties.currentPlayerCapacity.ToString("N0"); // 무게 텍스트 업데이트       
    }

    private void DecreaseWeight(float weight)
    {
        haveCapacity -= weight;       
        capacitySlider.value = haveCapacity; // 슬라이더 바에 무게 반영
        weightText.text = (haveCapacity).ToString("N0") + " / " + GameManager.Instance.playerProperties.currentPlayerCapacity.ToString("N0"); // 무게 텍스트 업데이트       
    }

    private void ExitWeight()
    {
        capacitySlider.value = haveCapacity; // 슬라이더 바에 무게 반영
        weightText.text = (haveCapacity).ToString("N0") + " / " + GameManager.Instance.playerProperties.currentPlayerCapacity.ToString("N0");
    }

    //public void StockInitializer()
    //{
    //    if (previousMonth != GameManager.Instance.curMonth)
    //    {
    //        for (int i = 0; i < goodsDB.Local.Count; i++)
    //        {
    //            goodsDB.Local[i].현지가격 = Sstock[i].price;
    //            goodsDB.Local[i].재고 = Sstock[i].stock;
    //        }

    //        // 마지막으로 업데이트된 달을 현재 달로 설정
    //        previousMonth = GameManager.Instance.curMonth;            
    //    }
    //}

    private void UpdateLocalPrefabText()
    {
        foreach (var prefab in instantiatedPrefabs)
        {
            var stockText = prefab.transform.GetChild(2).GetComponent<Text>();
            var priceText = prefab.transform.GetChild(3).GetComponent<Text>();

            // Find the corresponding goods in goodsDB
            var goodsName = prefab.transform.GetChild(0).GetComponent<Text>().text;
            var goods = goodsDB.Local.Find(g => g.특산물 == goodsName);

            // FixedUpdate the prefab text with goods information
            if (goods != null)
            {
                stockText.text = "재고: " + goods.재고.ToString();
                priceText.text = goods.현지가격.ToString();
            }
        }
    }

    public void initStock()
    {
        for (int i = 0; i < goodsDB.Local.Count; i++)
        {
            Sstock[i].stock = goodsDB.Local[i].재고;
            Sstock[i].price = goodsDB.Local[i].현지가격;
        }
    }

    // 거래 버튼 클릭시
    public void Trade()
    {
        if (GameManager.Instance.money > -totalCost && haveCapacity <= (capacitySlider.maxValue + 5000))
            GameManager.Instance.money += totalCost;
        else
            return;

        //foreach (GameObject prefab in newgoods)
        //{
        //    if (prefab.CompareTag("LocalPrefab"))
        //    {
        //        Text gnameText = prefab.transform.GetChild(0).GetComponent<Text>();
        //        string selectedGoodsName = gnameText.text;

        //        Text stock = prefab.transform.GetChild(2).GetComponent<Text>();
        //        int currentStock = int.Parse(stock.text.Split(':')[1]);

        //        Text price = prefab.transform.GetChild(3).GetComponent<Text>();
        //        int Price = int.Parse(price.text);

        //        addInventory(selectedGoodsName, currentStock, Price);
                
        //    }
        //    Destroy(prefab);
        //    totalCost.ToString();
        //}

        for(int i = newgoods.Count-1; i >= 0; i--)
        {
            Debug.Log("check");
            if (newgoods[i].CompareTag("LocalPrefab"))
            {
                Text gnameText = newgoods[i].transform.GetChild(0).GetComponent<Text>();
                string selectedGoodsName = gnameText.text;

                Text stock = newgoods[i].transform.GetChild(2).GetComponent<Text>();
                int currentStock = int.Parse(stock.text.Split(':')[1]);

                Text price = newgoods[i].transform.GetChild(3).GetComponent<Text>();
                int Price = int.Parse(price.text);

                addInventory(selectedGoodsName, currentStock, Price);
                GameObject Removed = newgoods[i];
                newgoods.RemoveAt(i);
                Destroy(Removed);
            }
            else if (newgoods[i].CompareTag("SalePrefab"))
            {
                GameObject Removed = newgoods[i];
                Destroy(Removed);
                newgoods.RemoveAt(i);
                //return;
            }
            totalCost.ToString();
        }

        totalCost = 0;
        costSum.transform.GetChild(1).GetComponent<Text>().text = totalCost.ToString();//거래 후 총 가격 초기화
        newgoods.Clear();
    }

    // 거래 종료 버튼 클릭시
    public void exitTrade() // 거래 종료 버튼이랑 연결된 함수
    {
        if (newgoods.Count != 0)
            return;

        PlayerMove.instance.ExitCol();
        exit = true;
        //foreach (GameObject prefab in instantiatedPrefabs)
        //{
        //    Debug.Log("check2");
        //    Destroy(prefab);
        //}
        for(int i = instantiatedPrefabs.Count-1; i >=0; i--)
        {
            Destroy(instantiatedPrefabs[i]);
        }
        instantiatedPrefabs.Clear();//거래창 나갈때 프리팹 초기화

        //foreach (GameObject prefab in newgoods)
        //    Destroy(prefab);
        for(int i= newgoods.Count-1; i>=0; i--)
        {
            Destroy(newgoods[i]);
        }
        newgoods.Clear();//거래창 나갈때 프리팹 초기화

        for (int i = 0; i < inventories.Count; i++)
        {
            if (inventories[i].activeSelf == false)
            {
                Destroy(inventories[i]);
                inventories.Remove(inventories[i]);
                i--;
            }
            else
                return;
        }

        ExitWeight();//구매 안하고 거래창 나갈때 무게 초기화

        totalCost = 0;
        costSum.transform.GetChild(1).GetComponent<Text>().text = totalCost.ToString();

        exit = false;
    }

    public void ReturnGoodsToLocal()
    {
        soundManager.PlaySFX("메뉴클릭");
        GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
        if (clickedObject == null) return;
    
        Text goodsNameText = clickedObject.transform.GetChild(0).GetComponent<Text>();
        if (goodsNameText == null) return;

        string goodsName = goodsNameText.text;
        Text tradeStockText = clickedObject.transform.GetChild(2).GetComponent<Text>();
        Text tradePriceText = clickedObject.transform.GetChild(3).GetComponent<Text>(); 
        int stockCount = int.Parse(tradeStockText.text.Split(':')[1]);
        int itemPrice = int.Parse(tradePriceText.text); 

        int weightIndex = goodsDB.Local.FindIndex(g => g.특산물 == goodsName);
        if (weightIndex < 0) return;
        float weight = goodsDB.Local[weightIndex].무게 * 1000;        

        if (clickedObject.CompareTag("SalePrefab")) 
        {
            var existingInventoryItem = inventories.Find(inventory => inventory.transform.GetChild(0).GetComponent<Text>().text == goodsName);

            if (existingInventoryItem != null)
            {                
                var inventoryStockText = existingInventoryItem.transform.GetChild(2).GetComponent<Text>();
                int currentInventoryStock = int.Parse(inventoryStockText.text.Split(':')[1]);
                currentInventoryStock++;
                inventoryStockText.text = "재고: " + currentInventoryStock.ToString();
                goodsDB.Local[weightIndex].재고++;
                existingInventoryItem.SetActive(true);
                IncreaseWeight(weight);                            
            }           
        }
        else if (clickedObject.CompareTag("LocalPrefab"))
        {            
            var localPrefab = instantiatedPrefabs.Find(prefab => prefab.transform.GetChild(0).GetComponent<Text>().text == goodsName);
            if (localPrefab != null)
            {
                var localStockText = localPrefab.transform.GetChild(2).GetComponent<Text>();
                int currentLocalStock = int.Parse(localStockText.text.Split(':')[1]);
                currentLocalStock++;
                localStockText.text = "재고: " + currentLocalStock.ToString();
                goodsDB.Local[weightIndex].재고++;
                localPrefab.SetActive(true);
                DecreaseWeight(weight);
            }
        }        
        stockCount--;
        if (stockCount > 0)
        {
            tradeStockText.text = "재고: " + stockCount.ToString();
        }
        else
        {            
            newgoods.Remove(clickedObject);
            Destroy(clickedObject);                        
        }        
        sumOfPurchaseAndSales();        
        if(newgoods.Count == 0)
        {
            totalCost = 0;
            costSum.transform.GetChild(1).GetComponent<Text>().text = totalCost.ToString();
        }
    }    
}