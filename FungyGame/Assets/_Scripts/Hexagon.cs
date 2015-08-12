using UnityEngine;
using System.Collections;
using System;

public delegate void HexagonEventHandler(object sender, EventArgs e);
public class Hexagon : MonoBehaviour
{
    public event HexagonEventHandler ClickEvent;
    protected virtual void OnHexagonClick(EventArgs e)
    {
        if (ClickEvent != null)
            ClickEvent(this, e);
    }

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    #region Hexagon positionning
    private int _posX = -1;
    public int X
    {
        get
        {
            return _posX;
        }
    }

    private int _posY = -1;
    public int Y
    {
        get
        {
            return _posY;
        }
    }

    private Renderer _renderer;
    public Renderer HexagonRenderer
    {
        get
        {
            return _renderer;
        }
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
    #endregion
    void OnMouseDown()
    {
        OnHexagonClick(new EventArgs());
    }
}
