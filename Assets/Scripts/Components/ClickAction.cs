using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ClickAction : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] public GameActivity context;
    private RawImage r;
    private Vector3[] fourCorners;
    private float width;
    private float height;
    private GridLayout gridLayout;
    private bool drag = false;
    public void OnPointerClick(PointerEventData eventData){
        Vector2 minimapCoords = GetCoordinatesOnMinimap(eventData.position);
        
        int tileMapX = (int)((minimapCoords.x*context.gameModel.COLS)/width);
        int tileMapY = (int)((minimapCoords.y*context.gameModel.ROWS)/height);
        if(tileMapX==context.gameModel.COLS) tileMapX--;
        if(tileMapY==context.gameModel.ROWS) tileMapY--;

        Vector3 worldPos = gridLayout.CellToWorld(new Vector3Int(tileMapX, tileMapY, 0));
        context.gameModel.GameCameraPos = new Vector3(worldPos.x, worldPos.y, -59);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
       drag = true;
    }

    public void OnPointerUp(PointerEventData eventData){
        drag = false;
    }

    public Vector2 GetCoordinatesOnMinimap(Vector2 eventDataPosition){
        return new Vector2(eventDataPosition.x - fourCorners[0].x, eventDataPosition.y - fourCorners[0].y);
    }
    // Start is called before the first frame update
    void Start()
    {
        fourCorners = new Vector3[4];
        r = this.GetComponent<RawImage>();
        r.rectTransform.GetWorldCorners(fourCorners);

        height = fourCorners[1].y - fourCorners[0].y;
        width = fourCorners[3].x - fourCorners[0].x;
 gridLayout = context.terrain.GetComponentInParent<GridLayout>();
        

        
    }

    // Update is called once per frame
    void Update()
    {
        if(drag){
            Vector2 minimapCoords = GetCoordinatesOnMinimap(Input.mousePosition);
           // Debug.Log(minimapCoords);
            int tileMapX = (int)((minimapCoords.x*context.gameModel.COLS)/width);
            int tileMapY = (int)((minimapCoords.y*context.gameModel.ROWS)/height);
            if(tileMapX==context.gameModel.COLS) tileMapX--;
            if(tileMapY==context.gameModel.ROWS) tileMapY--;

            Vector3 worldPos = gridLayout.CellToWorld(new Vector3Int(tileMapX, tileMapY, 0));
            context.gameModel.GameCameraPos = new Vector3(worldPos.x, worldPos.y, -59);
        }
    }
}
