using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections; 
using System.Collections.Generic;


public class GridCreator : MonoBehaviour
{
    public GameObject cellPrefab; // Prefab of the cell
    private int gridSize = 0; // Size of the grid
    private InputField inputField; // Input field to get the grid size

    /* Start function of the grid*/
    void Start()
    {
        //make cellPrefab and text of it invisible
        cellPrefab.GetComponent<SpriteRenderer>().enabled = false;
        cellPrefab.GetComponentInChildren<TextMeshPro>().enabled = false;

        //get the input field
        GameObject inputFieldObj = GameObject.Find("InputField");
        inputField = inputFieldObj.GetComponent<InputField>();

        //add listener to the input field
        inputField.onEndEdit.AddListener(OnInputFieldEndEdit);

        //create the grid
        if(gridSize > 0)
            CreateGrid();
    }

    /* Recursive function to check the adjacent cells */
    public void CheckCell(int cellIndexPrev, int cellIndexCurrent, int counter, HashSet<int> cellIndexVisited)
    {
        //add current cell to the visited cells
        cellIndexVisited.Add(cellIndexCurrent);
        //if the current cell is connected to 3 or more cells, release all the visited cells
        if(counter >= 3)
        {
            foreach(int cellIndex in cellIndexVisited)
            {
                //release the cell
                CellButton cellButton = GameObject.Find("Cell_" + cellIndex).GetComponent<CellButton>();
                cellButton.ReleaseCell();
            }
            return;
        }

        //possible the adjacent cells
        int cellIndexUp = (cellIndexCurrent + 1) % gridSize != 0 ? cellIndexCurrent + 1 : -1;
        int cellIndexDown = (cellIndexCurrent - 1) % gridSize != gridSize-1 ? cellIndexCurrent - 1 : -1;
        int cellIndexRight = cellIndexCurrent + gridSize;
        int cellIndexLeft = cellIndexCurrent - gridSize;
        int[] adjacentIndexes = {cellIndexUp, cellIndexDown, cellIndexRight, cellIndexLeft};   

        //check the adjacent cells
        foreach(int adjacentCellIndex in adjacentIndexes)
        {
            //if the adjacent cell is in the grid
            if(adjacentCellIndex >= 0 && adjacentCellIndex < gridSize * gridSize)
            {
                //get the adjacent cell
                CellButton adjacentCell = GameObject.Find("Cell_" + adjacentCellIndex).GetComponent<CellButton>();

                //if the adjacent cell is in the grid and it is not the previous cell
                if(adjacentCell != null && adjacentCell.CellIndex != cellIndexPrev)
                {
                    //if the adjacent cell is clicked
                    if(adjacentCell.IsClicked)
                    {
                        //Check the adjacent cell
                        CheckCell(cellIndexCurrent, adjacentCell.CellIndex, ++counter, cellIndexVisited);
                    }
                }
            }
        }

    }

    /*Callback function to handle changing value of inputField*/
    private void OnInputFieldEndEdit(string submittedText)
    {
        //get grid size from the input field
        int newGridSize = int.Parse(submittedText);
        //if the grid size is valid, create the grid
        if(newGridSize > 0 && newGridSize != gridSize)
        {
            //set the grid size
            gridSize = newGridSize;
            //delete the previous grid
            DeleteGrid();
            //create the new grid
            CreateGrid();
        } 
    }

    /*Function to create grid*/
    void CreateGrid()
    {
        //get the screen size
        float screenHeight = Camera.main.orthographicSize * 2;
        float screenWidth = screenHeight * Camera.main.aspect;

        //calculate the cell size and the grid size
        float cellSize = Mathf.Min(screenWidth / gridSize, screenHeight / gridSize);
        float gridWidth = cellSize * gridSize;
        float gridHeight = cellSize * gridSize;
        
        //calculate the offset
        float offsetX = (screenWidth - gridWidth) / 2;
        float offsetY = (screenHeight - gridHeight) / 2;
        
        //calculate the initial position of the grid
        float init_X = -screenWidth / 2 + offsetX + cellSize / 2;
        float init_Y = -screenHeight / 2 + offsetY + cellSize / 2;


        //create the grid
        for (int i = 0; i < gridSize; i++) // i = x
        {
            for (int j = 0; j < gridSize; j++) // j = y
            {
                //create the cell
                GameObject cell = Instantiate(cellPrefab, transform);
                cell.GetComponent<SpriteRenderer>().enabled = true;
                
                //set the cell index and the cell size
                CellButton cellButton = cell.GetComponent<CellButton>();
                cellButton.CellIndex = i * gridSize + j; // Set the cell index
                cellButton.CellSize = (int)cellSize;
                cell.name = "Cell_" + cellButton.CellIndex;

                //set the position and the scale of the cell
                float xPos = init_X + (i * cellSize);
                float yPos = init_Y + (j * cellSize);
                cell.transform.position = new Vector3(xPos, yPos, 0f);                
                cell.transform.localScale = new Vector3(cellSize, cellSize, 1f);

            }
        }
    }

    /*Function to delete grid*/
    void DeleteGrid()
    {
        //delete all the children of the grid
        foreach (Transform child in transform)
        {
            //delete the child
            Destroy(child.gameObject);
        }
    }
}
