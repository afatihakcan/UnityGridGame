using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections; 
using System.Collections.Generic;


public class CellButton : MonoBehaviour
{
    public int CellIndex{get; set;} // Index of the cell
    public int CellSize{get; set;} // Size of the cell
    public bool IsClicked{get; set;} // Is the cell clicked?
    private GridCreator gridCreator; // Reference to the GridCreator script
    private TextMeshPro tmpText;

    /* Start function of the cell */
    private void Start()
    {
        gridCreator = FindObjectOfType<GridCreator>();
        tmpText = GetComponentInChildren<TextMeshPro>();
        tmpText.text = "X";

        ReleaseCell();    
    }

    /* Function to release the clicked cell */
    public void ReleaseCell()
    {
        IsClicked = false;
        tmpText.enabled = false;
    }

    /* Callback function to handle the click on the cell*/
    public void OnMouseUp() {
        //if the cell is not the cell prefab
        if(gameObject.name != "Cell")
        {
            //toggle the cell click
            IsClicked = !IsClicked;

            //if the cell is clicked
            if(IsClicked){
                //make the "X" visible
                tmpText.enabled = true;
                //create a hashset to store the visited cells
                HashSet<int> cellIndexVisited = new HashSet<int>();
                //create a counter to count the number of connected cells
                int counter = new int();
                //check the cell
                gridCreator.CheckCell(CellIndex, CellIndex, ++counter, cellIndexVisited); // Check the cell
            }
                
            else{
                //make the "X" invisible
                tmpText.enabled = false;
            }   
        }
    }

}
