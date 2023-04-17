using Assets.Scripts.Managers;
using Models;
using UnityEngine;
using UnityEngine.UI;

public class UIMinimap : MonoBehaviour
{

    public Collider minimapBoundingBox;
    public Image minimap;
    public Image arrow;
    public Text mapName;

    private Transform playerTransform;
    // Use this for initialization
    void Start()
    {
        MinimapManager.Instance.minimap = this;
        this.UpdateMap();
    }

    public void UpdateMap()
    {
        this.mapName.text = User.Instance.CurrentMapData.Name;
        // 加载小地图图片
        this.minimap.overrideSprite = MinimapManager.Instance.LoadCurrentMinimap();
        // image组件加载新的sprite后，需要设置原大小，否则可能新进入的小地图，图片大小还是原来的图片大小
        this.minimap.SetNativeSize();
        this.minimap.transform.localPosition = Vector3.zero;
        this.minimapBoundingBox = MinimapManager.Instance.MinimapBoundingBox;

        this.playerTransform = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform == null)
        {
            // 缓存玩家控制的GameObject的Transform，减少对单例的引用，节约性能
            this.playerTransform = MinimapManager.Instance.PlayerTransform;
        }


        if (minimapBoundingBox == null || playerTransform == null)
        {
            return;
        }
        float realWidth = minimapBoundingBox.bounds.size.x;
        float realHeight = minimapBoundingBox.bounds.size.z;

        float relaX = playerTransform.position.x - minimapBoundingBox.bounds.min.x;
        float relaY = playerTransform.position.z - minimapBoundingBox.bounds.min.z;

        float pivotX = relaX / realWidth;
        float pivotY = relaY / realHeight;

        // 图片的pivot是显示中点，将玩家位置转化为图片中点，可以减少运算
        this.minimap.rectTransform.pivot = new Vector2(pivotX, pivotY);
        this.minimap.rectTransform.localPosition = Vector2.zero;
        // 将角色的y轴旋转赋值给小地图箭头
        this.arrow.transform.eulerAngles = new Vector3(0, 0, -playerTransform.eulerAngles.y);
    }
}
