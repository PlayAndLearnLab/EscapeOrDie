using UnityEngine;

public class FloorVisibilityController : MonoBehaviour
{
    public GameObject firstFloorFurnitureGroup;
    public GameObject secondFloorFurnitureGroup;


    // Call this when the player changes floor
    public void SetFloor(int floorNumber)
    {
        bool isSecondFloor = (floorNumber == 2);

        firstFloorFurnitureGroup.SetActive(!isSecondFloor);
        secondFloorFurnitureGroup.SetActive(isSecondFloor);
    }
}
