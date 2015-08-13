using UnityEngine;
using System.Collections;
using System;

public delegate void HexagonEventHandler(object sender, EventArgs e, int clickID);
public class Hexagon : MonoBehaviour
{
    #region CLick event
    public event HexagonEventHandler ClickEvent;
    protected virtual void OnHexagonClick(EventArgs e, int clickID)
    {
        if (ClickEvent != null)
            ClickEvent(this, e, clickID);
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
            OnHexagonClick(new EventArgs(), 0);
        if (Input.GetMouseButtonDown(1))
            OnHexagonClick(new EventArgs(), 1);
    }
    #endregion

    public TreeClass HexTree { get; set; }

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    private int _posX = -1;
    /// <summary>
    /// X position of the hexagon in the map grid
    /// </summary>
    public int X
    {
        get { return _posX; }
    }

    private int _posY = -1;
    /// <summary>
    /// Y position of the hexagon in the map grid
    /// </summary>
    public int Y
    {
        get { return _posY; }
    }

    private Renderer _renderer;
    /// <summary>
    /// Readonly hexagon's renderer
    /// </summary>
    public Renderer HexagonRenderer
    {
        get { return _renderer; }
    }

    /// <summary>
    /// Set the (x,y) information about the position of the tile
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetPosition(int x, int y)
    {
        _posX = x;
        _posY = y;
    }
}
