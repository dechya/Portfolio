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

    //���� ����
    private int totalCost;
    private int purCost;
    private int saleCost;

    public UnityEvent onCargoWindowEnabled;

    private string selectedProvince;

    [Header("��� ���� Ư�깰")]
    [SerializeField] private GameObject preview;

    [Header("�� â�� ������ ������ ����Ʈ")]
    public List<GameObject> instantiatedPrefabs = new List<GameObject>();
    public List<GameObject> newgoods = new List<GameObject>();
    public List<GameObject> inventories = new List<GameObject>();

    [Header("ó�� ��� ���� �� �迭")]
    public SavedStocks[] Sstock = new SavedStocks[90];

    [Header("����ȭ��")]
    [SerializeField] private GameObject TradeChang; // ����ȭ�� �������� ������ â
    [SerializeField] private GameObject LocalTradePrefab; // ����ȭ�� ������

    [Header("�ŷ�ȭ��")]
    [SerializeField] private GameObject PurchaseChang; // �ŷ�ȭ�� �������� ������ â
    [SerializeField] private GameObject purchasePrefab; // �ŷ�ȭ�� ������
    public GameObject salePrefab; // �ŷ�â�� �����Ǵ� �Ǹ� ������
    public GameObject costSum; // ����, �ǸŰ��� �ջ�    

    [Header("����ȭ��")]
    public GameObject inventransfrom; // ����ȭ�� �������� ������ â
    public GameObject invenPrefab; // ����ȭ�� ������

    [Header("���緮 �����̴�")]
    public Slider capacitySlider;
    [HideInInspector] public float haveCapacity;
    [HideInInspector] public bool disCount = false;            
    [HideInInspector] public bool halfSpeedApplied = false;                 

    [Header("���� �ؽ�Ʈ")]
    public Text weightText;

    [Header("�ʱ�ȭ�� ��")]
    public bool exit = false;
    [HideInInspector] public bool isUp = false;

    public ParticleImage particleImage;

    int previousMonth;// �������� ������ ����
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
        weightText.text = (haveCapacity).ToString("N0") + " / " + GameManager.Instance.playerProperties.currentPlayerCapacity.ToString("N0"); // ���� �ؽ�Ʈ ������Ʈ       
    }

    // ������ �����տ� ��ư �߰�
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

    // ����ȭ���� Ư�깰 �߰�
    private void DisplaylocalPrefab(string prvName)
    {        
        //foreach (GameObject prefab in instantiatedPrefabs)
        //    instantiatedPrefabs.Remove(prefab);

        //instantiatedPrefabs.Clear();

        for (int i = 0; i < goodsDB.Local.Count; i++)
        {

            if (prvName == goodsDB.Local[i].�� && goodsDB.Local[i].��� > 0)
            {
                GameObject trade = Instantiate(LocalTradePrefab, TradeChang.transform);
                Debug.Log("tarde : " + trade.gameObject.name);
                instantiatedPrefabs.Add(trade);

                Transform prefabTransform = trade.transform;

                Text goodsNameText = prefabTransform.GetChild(0).GetComponent<Text>();
                goodsNameText.text = goodsDB.Local[i].Ư�깰;

                Text stockText = prefabTransform.GetChild(2).GetComponent<Text>();
                stockText.text = "���: " + goodsDB.Local[i].���.ToString();

                Text priceText = prefabTransform.GetChild(3).GetComponent<Text>();
                priceText.text = goodsDB.Local[i].��������.ToString();
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

    // for���̳� if���̳� �ڷ�ƾ�Ἥ ���� �ѹ��� ����
    // ����ȭ�� ��ư Ŭ����    
    public void OnPrefabButtonClick()
    {
        soundManager.PlaySFX("�޴�Ŭ��");
        if ((haveCapacity) < capacitySlider.maxValue)
        {
            Button clickedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();//Ŭ���� ��ư�� ������ ��������
            Text buttonText = clickedButton.GetComponentInChildren<Text>();
            Text stockText = clickedButton.transform.GetChild(2).GetComponent<Text>();

            string selectedGoodsName = buttonText.text; // Ŭ���� ��ư�� �ؽ�Ʈ�� ��������

            int dbindex = goodsDB.Local.FindIndex(g => g.Ư�깰 == selectedGoodsName);
            float weight = (goodsDB.Local[dbindex].���� * 1000); //Ŭ���� ��ư�� Ư�깰�̸��� goodsDB���� ã�Ƽ� �ε������� ��������
            IncreaseWeight(weight);
            goodsDB.Local[dbindex].��� -= 1;//Ŭ���� Ư�깰�� ��� 1���ҽ�Ű��

            stockText.text = "���:" + goodsDB.Local[dbindex].���.ToString();//Ư�깰�� ��� �ؽ�Ʈ�� ǥ��

            if (goodsDB.Local[dbindex].��� <= 0)
                clickedButton.gameObject.SetActive(false);
            
            displayPurchasePrefabText(buttonText.text);//������ �����ո�Ͽ� �ؽ�Ʈ �߰�

        }
        else
            return;
        
        addButtonToTradePrefab();
    }
 

    // ����ȭ�� Ư�깰 �߰�
    public void addInventory(string selectedGoods, int stock, int price)
    {
        // �ߺ� üũ
        GameObject existingInventory = inventories.Find(inv => inv.transform.GetChild(0).GetComponent<Text>().text == selectedGoods);

        if (existingInventory != null)
        {
            // �̹� �ִ� Ư�깰�� ��� ������Ʈ
            Text stockText = existingInventory.transform.GetChild(2).GetComponent<Text>();
            int currentStock = int.Parse(stockText.text.Split(':')[1]);

            currentStock += stock;
            stockText.text = "���: " + currentStock.ToString();
            existingInventory.SetActive(true);
            return; // �ߺ��� Ư�깰�̹Ƿ� �Լ� ����
        }

        // �ߺ����� �ʴ� Ư�깰�� ��� ���ο� �κ��丮 ������ ����
        GameObject inventoryItem = Instantiate(invenPrefab, inventransfrom.transform);

        Transform invenTransform = inventoryItem.transform;
        Transform ptransform = purchasePrefab.transform;

        Text gnameText = invenTransform.GetChild(0).GetComponent<Text>();
        gnameText.text = selectedGoods;

        Text stocktext = invenTransform.GetChild(2).GetComponent<Text>();
        stocktext.text = "���: " + stock.ToString();

        Text costText = invenTransform.GetChild(3).GetComponent<Text>();
        costText.text = price.ToString();

        // ������ �ؽ�Ʈ�� costtext�� ����

        // ���� ������ Ư�깰 ������ ã�Ƽ� �κ��丮 �����ۿ� ����
        Button saleButton = inventoryItem.GetComponent<Button>();        
        saleButton.onClick.AddListener(() => OnSaleButtonClick(inventoryItem, gnameText.text));
        inventories.Add(inventoryItem);        
    }

    // ����ȭ�� ��ư Ŭ����
    public void OnSaleButtonClick(GameObject saleObject, string selectedGoodsName)
    {
        soundManager.PlaySFX("�޴�Ŭ��");

        Button clickedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        GameObject existingGoods = newgoods.Find(g => g.transform.GetChild(0).GetComponent<Text>().text == selectedGoodsName && g.CompareTag("SalePrefab"));
        Text invnenstockText = clickedButton.transform.GetChild(2).GetComponent<Text>();

        int dbindex = goodsDB.Local.FindIndex(g => g.Ư�깰 == selectedGoodsName);
        float weight = (goodsDB.Local[dbindex].���� * 1000); //Ŭ���� ��ư�� Ư�깰�̸��� goodsDB���� ã�Ƽ� �ε������� ��������
        DecreaseWeight(weight);

        // �κ��丮 ������ ��� ������Ʈ
        Text InvnenstockText = saleObject.transform.GetChild(2).GetComponent<Text>();
        int Invenstock = int.Parse(InvnenstockText.text.Split(':')[1]);

        if (Invenstock > 0)
        {            
            Invenstock -= 1;
            InvnenstockText.text = "���:" + Invenstock.ToString(); // �κ��丮�� Ư�깰 ��� ����

        }
        if (Invenstock == 0)
        {
            saleObject.SetActive(false);
            //Destroy(saleObject); // ��� 0�̸� �κ��丮 ������ ����
            //inventories.Remove(saleObject);
        }

        //�Ǹ� ��ǰ�� ��� ������Ʈ�ϰ� �Լ� ����
        if (existingGoods != null)
        {
            Text stockText = existingGoods.transform.GetChild(2).GetComponent<Text>();
            int currentStock = int.Parse(stockText.text.Split(':')[1]);
            currentStock += 1;
            stockText.text = "���:" + currentStock.ToString();

            // �� ���� ������Ʈ
            sumOfPurchaseAndSales();

            return;
        }

        // ���ο� �Ǹ� ��ǰ ����
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
        stockTextNew.text = "���:" + newStock.ToString();

        Button saleButton = saleObject.GetComponent<Button>();
        saleButton.onClick.RemoveAllListeners();
        saleButton.onClick.AddListener(ReturnGoodsToLocal);        
        newgoods.Add(saleObject);
        sumOfPurchaseAndSales();        
    }

    //��͵����� ���� ������Ʈ
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
        var goods = goodsDB.Local.Find(item => item.Ư�깰 == goodsName);
        if (goods != null)
            return goods.Ư�깰���; // Ư�깰�� ����� ��ȯ
        else
        {
            // Ư�깰�� ã�� �� ���� ��� �⺻���� ��ȯ
            return "�Ϲ�";
        }
    }
    public float GetMultiplierByRarity(string rarityLevel)
    {
        switch (rarityLevel)
        {
            case "�Ϲ�":
                isUp = true;
                return 1.2f;

            case "���":
                isUp = true;
                return 1.4f;

            case "���":
                isUp = true;
                return 1.6f;

            default:
                break;
        }
        return 1.0f;
    }
    public void displayPurchasePrefabText(string selectedGoodsName)// ������ �����ո�Ͽ� �ؽ�Ʈ �߰�
    {
        GameObject ClickedObject = EventSystem.current.currentSelectedGameObject;
        int dbIndex = goodsDB.Local.FindIndex(g => g.Ư�깰 == selectedGoodsName);

        GameObject existingGoods = newgoods.Find(g => g.transform.GetChild(0).GetComponent<Text>().text == selectedGoodsName && g.CompareTag("LocalPrefab"));

        if (existingGoods != null)
        {
            Text stockText = existingGoods.transform.GetChild(2).GetComponent<Text>();
            int currentStock = int.Parse(stockText.text.Split(':')[1]);
            currentStock++;
            stockText.text = "���:" + currentStock.ToString();

            // �� ���� ������Ʈ
            sumOfPurchaseAndSales();

            return; // �ߺ��� ��ǰ�̹Ƿ� �Լ� ����
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
        stockTextNew.text = "���:" + newStock.ToString();

        newgoods.Add(newGoods);

        // �� ���� ������Ʈ
        sumOfPurchaseAndSales();
    }

    // �ŷ�â ����, �ǸŰ��� �ջ�
    public void sumOfPurchaseAndSales() //����,�ǸŰ����ջ�
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
        capacitySlider.value = haveCapacity; // �����̴� �ٿ� ���� �ݿ�
        weightText.text = (haveCapacity).ToString("N0") + " / " + GameManager.Instance.playerProperties.currentPlayerCapacity.ToString("N0"); // ���� �ؽ�Ʈ ������Ʈ       
    }

    private void DecreaseWeight(float weight)
    {
        haveCapacity -= weight;       
        capacitySlider.value = haveCapacity; // �����̴� �ٿ� ���� �ݿ�
        weightText.text = (haveCapacity).ToString("N0") + " / " + GameManager.Instance.playerProperties.currentPlayerCapacity.ToString("N0"); // ���� �ؽ�Ʈ ������Ʈ       
    }

    private void ExitWeight()
    {
        capacitySlider.value = haveCapacity; // �����̴� �ٿ� ���� �ݿ�
        weightText.text = (haveCapacity).ToString("N0") + " / " + GameManager.Instance.playerProperties.currentPlayerCapacity.ToString("N0");
    }

    //public void StockInitializer()
    //{
    //    if (previousMonth != GameManager.Instance.curMonth)
    //    {
    //        for (int i = 0; i < goodsDB.Local.Count; i++)
    //        {
    //            goodsDB.Local[i].�������� = Sstock[i].price;
    //            goodsDB.Local[i].��� = Sstock[i].stock;
    //        }

    //        // ���������� ������Ʈ�� ���� ���� �޷� ����
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
            var goods = goodsDB.Local.Find(g => g.Ư�깰 == goodsName);

            // FixedUpdate the prefab text with goods information
            if (goods != null)
            {
                stockText.text = "���: " + goods.���.ToString();
                priceText.text = goods.��������.ToString();
            }
        }
    }

    public void initStock()
    {
        for (int i = 0; i < goodsDB.Local.Count; i++)
        {
            Sstock[i].stock = goodsDB.Local[i].���;
            Sstock[i].price = goodsDB.Local[i].��������;
        }
    }

    // �ŷ� ��ư Ŭ����
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
        costSum.transform.GetChild(1).GetComponent<Text>().text = totalCost.ToString();//�ŷ� �� �� ���� �ʱ�ȭ
        newgoods.Clear();
    }

    // �ŷ� ���� ��ư Ŭ����
    public void exitTrade() // �ŷ� ���� ��ư�̶� ����� �Լ�
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
        instantiatedPrefabs.Clear();//�ŷ�â ������ ������ �ʱ�ȭ

        //foreach (GameObject prefab in newgoods)
        //    Destroy(prefab);
        for(int i= newgoods.Count-1; i>=0; i--)
        {
            Destroy(newgoods[i]);
        }
        newgoods.Clear();//�ŷ�â ������ ������ �ʱ�ȭ

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

        ExitWeight();//���� ���ϰ� �ŷ�â ������ ���� �ʱ�ȭ

        totalCost = 0;
        costSum.transform.GetChild(1).GetComponent<Text>().text = totalCost.ToString();

        exit = false;
    }

    public void ReturnGoodsToLocal()
    {
        soundManager.PlaySFX("�޴�Ŭ��");
        GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
        if (clickedObject == null) return;
    
        Text goodsNameText = clickedObject.transform.GetChild(0).GetComponent<Text>();
        if (goodsNameText == null) return;

        string goodsName = goodsNameText.text;
        Text tradeStockText = clickedObject.transform.GetChild(2).GetComponent<Text>();
        Text tradePriceText = clickedObject.transform.GetChild(3).GetComponent<Text>(); 
        int stockCount = int.Parse(tradeStockText.text.Split(':')[1]);
        int itemPrice = int.Parse(tradePriceText.text); 

        int weightIndex = goodsDB.Local.FindIndex(g => g.Ư�깰 == goodsName);
        if (weightIndex < 0) return;
        float weight = goodsDB.Local[weightIndex].���� * 1000;        

        if (clickedObject.CompareTag("SalePrefab")) 
        {
            var existingInventoryItem = inventories.Find(inventory => inventory.transform.GetChild(0).GetComponent<Text>().text == goodsName);

            if (existingInventoryItem != null)
            {                
                var inventoryStockText = existingInventoryItem.transform.GetChild(2).GetComponent<Text>();
                int currentInventoryStock = int.Parse(inventoryStockText.text.Split(':')[1]);
                currentInventoryStock++;
                inventoryStockText.text = "���: " + currentInventoryStock.ToString();
                goodsDB.Local[weightIndex].���++;
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
                localStockText.text = "���: " + currentLocalStock.ToString();
                goodsDB.Local[weightIndex].���++;
                localPrefab.SetActive(true);
                DecreaseWeight(weight);
            }
        }        
        stockCount--;
        if (stockCount > 0)
        {
            tradeStockText.text = "���: " + stockCount.ToString();
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