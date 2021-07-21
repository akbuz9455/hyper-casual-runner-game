using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public static playerController Current;//diğer sınıflardan bu sınıfa erişmek için statik şekilde classı oluşturduk


    public float limitX; //x düzleminde gidebileceği limit
    public float xSpeed;//karakterin sağa sola ne kadar hızla gideceğini tutacak
    public float runningSpeed;
    private float _currentRunningSpeed;

    public Animator animator; //karakterin animasyonunu tutacağız

    public GameObject ridingCylinderPrefab;
    public List<ridingCylinder> cylinders; //ayağımız altındaki silindirleri tutmak için hazırladık

    private float _creatingBridgeTimer;
    private bool _spawningBridge; //true ise şuan köprü oluşturuyor demektir.
    public GameObject bridgePiecePrefab; // köprü oluşturmak için kullanacağı köprü parçalarını buradan çekeceğiz
    private BridgeSpawner _bridgeSpawener; //köprüyü oluştururken başlangıç ve bitiş referans noktalarına ihtiyacımız olacak
                                           //bu yüzden ilgili bridgeSpawnerlara erişmemiz gerekiyor

    private float scoreTimer = 0;
    private bool _finished;

    private float _lastTouchedX;



    void Start()
    {
        Current = this;
        //     _currentRunningSpeed = runningSpeed;

     
    }

    
    void Update()
    {
             if (LevelController.Current==null || !LevelController.Current.gameActive)//herhangi bir level kontroller yoksa
            {
                 return;
            }
     
            float newX = 0; // karakterin x düzleminde yeni konumu
            float touchXDelta = 0; //mause sağa sola ne kadar götürdüğünü tutmak için hazırladık

            if (Input.touchCount > 0) //eğer ekrana basılıyor ve hareket ettiriliyorsa parmak
             {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                _lastTouchedX = Input.GetTouch(0).position.x;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                touchXDelta = 10 * (_lastTouchedX - Input.GetTouch(0).position.x) / Screen.width;
                _lastTouchedX = Input.GetTouch(0).position.x;
            }

             }
             else if (Input.GetMouseButton(0))//fare sol tık basılıysa
             {
                 touchXDelta = Input.GetAxis("Mouse X");
             }




         newX = transform.position.x + xSpeed * -touchXDelta * Time.deltaTime; //kaydırdıkdan sonraki yeni pozisyonu
          newX = Mathf.Clamp(newX, -limitX, limitX);//eğer yeni pozisyon limitx yada -limitx sınırlarını geçerse
        //limitx değerini döndür geçmezse kendi değeri dönecektir.

        //karakteri hareket ettiriyor her saniye
        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z + _currentRunningSpeed * Time.deltaTime);
            transform.position = newPosition;


            if (_spawningBridge)//köprü oluşturuluyor mu ?
            {
                _creatingBridgeTimer -= Time.deltaTime;
                if (_creatingBridgeTimer < 0)
                {
                    _creatingBridgeTimer = 0.01f;//silindir oluşturma süresi 
                    IncrementCylinderVolume(-0.01f);
                    GameObject createBridgePiece = Instantiate(bridgePiecePrefab); //köprü parçasını oluşturduk

                    //doğru konuma getirmeli ve doğru şekilde döndürmeliyiz.
                    Vector3 direction = _bridgeSpawener.endReferance.transform.position - _bridgeSpawener.startReferance.transform.position;
                    //yön vektörü elde edildi
                    float distance = direction.magnitude; //iki vectör arasi uzunluk

                    direction = direction.normalized; //hesaplanabilmesi için aradaki mesafeyi normal hale getiriyoruz


                    createBridgePiece.transform.forward = direction; //yönlendirme işini yapıyoruz.

                    //iki mesafe arası karakterimizin ne kadar ilerlediğini z noktasından bulacağız ve buna göre oluşturdumuz objeyi ilerleteceğiz.
                    float characterDistance = transform.position.z - _bridgeSpawener.startReferance.transform.position.z;
                    //pozisyonu sınırladık
                    characterDistance = Mathf.Clamp(characterDistance, 0, distance);
                    //oluşturdumuz objenin yeni pozisyonunu tutması için vectör3 değişkeni oluşturduk.
                    Vector3 newPiecePosition = _bridgeSpawener.startReferance.transform.position + direction * characterDistance;
                    //başlangıç referans noktası pozisyonuna eriştik aynı yönde karakterin uzaklığı kadar ilerlettik

                    newPiecePosition.x = transform.position.x; //karakterin sağa sola gitme durumunda pparçalarda sağa sola gitsin.
                    createBridgePiece.transform.position = newPiecePosition; //hesaplamalardan sonra ürettimiz köprünün pozisyonunu
                                                                             //oluşturduğumuz değere eşitledik 


                    if (_finished)
                    {
                        scoreTimer -= Time.deltaTime;
                        if (scoreTimer < 0)
                        {
                            scoreTimer = 0.3f;
                            LevelController.Current.ChangeScore(1);
                        }
                    }
                }
            }

        

      






}


    public void ChangeSpeed(float value) {
        _currentRunningSpeed = value;
    }

    private void OnTriggerEnter(Collider other) //temas sağlarsa sadece temas anını baz alır
    {
        if (other.tag == "addCylinder") //büyüme silindirlerine çarpıyor mu
        {
            IncrementCylinderVolume(0.2f);
            Destroy(other.gameObject); //çarptığı nesneyi yokedecek.
        }

        else if (other.tag == "SpawnBridge")
        {
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }

        else if (other.tag == "StopSpawnBridge")
        {
            StoppawningBridge();
            if (_finished)
            {
                Debug.Log("Temas SağlandıStopSpawnBridge ve  Finiş");
                LevelController.Current.FinishGame(); 
            }

        }
        else if (other.tag == "Finish")
        {
            Debug.Log("Temas Sağlandı Finiş");
            _finished = true;
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());

        }

    }

    private void OnTriggerStay(Collider other) //temas sağlarsa sadece temas anını baz alır
    {
        if (other.tag == "Trap")
        {
            IncrementCylinderVolume(-Time.fixedDeltaTime);
          
        }

    }


    public void IncrementCylinderVolume(float value) //silindir büyütme fonksiyonumuz
    {
        if (cylinders.Count==0) //eğer ayamın altında silindir yoksa
        {
            if (value>0) //yani azaltmaya çalışılmıyorsa yeni silindir eklenecekse
            {
                CreateCylinder(value);
            }
            else if (value<0)
            {
                //level bitimi
                if (_finished)
                {
                    LevelController.Current.FinishGame();
                }
                else
                {

                    Die();
                }

                //gameOver
            }
        }
        else
        {
            cylinders[cylinders.Count - 1].IncrementCylinderVolume(value);//en son yani en alttaki silindirin boyutunu büyütüyoruz.
        }

    } 

    public void Die()
    {
        animator.SetBool("running", false); //animasyon işini hallettik
        animator.SetBool("dead",true);
        gameObject.layer = 8; //çakışmayan bir layere geçerek yere düşsün
        Camera.main.transform.SetParent(null); //kamera öleni takip etmesin
        LevelController.Current.GameOver();
    }
    public void CreateCylinder(float value)
    {
        ridingCylinder createdCylinder = Instantiate(ridingCylinderPrefab,transform).GetComponent<ridingCylinder>();
        //burada instantiate  fonksiyonu ile yeni gameobject oluşturduk.
        //transform vererekden karakterimizin child'i haline getirdik
        //ve ridingCylinder klassından liste olduğu için ridingcylinder companentini çekdik
        cylinders.Add(createdCylinder); //silindiri listeye ekledik
        createdCylinder.IncrementCylinderVolume(value);//silindirin boyutunu güncelledik

    }

    public void DestroyCylinder(ridingCylinder cylinder) //silindiri silme yeri
    {
        cylinders.Remove(cylinder); //önce listeden temizliyoruz
        Destroy(cylinder.gameObject); // daha sonra gameobjesini yokediyoruz
    }

    public void StartSpawningBridge(BridgeSpawner spawner) //köprü oluşturmaya başliyoruz
    {

        _bridgeSpawener = spawner; //köprü nesnesini aldımız parametre ile referans noktalarına eriştirdik

        _spawningBridge = true;//şuan köprü oluşturuluyor
    }

    public void StoppawningBridge() //köprü oluşturmaya başliyoruz
    {


        _spawningBridge = false;//şuan köprü oluşturmuyor
    }
}
