using UnityEngine;
using System.Collections;

public class PerlinNoise
{
	private float realWid;
	private float realHei;
	private float tileSize;
	private int wid;
	private int hei;
	public float[,] noise {
		get;
		private set;
	}
	
	private Map map;
	
	public PerlinNoise (Map map)
	{
		this.map = map;
	}
	
	public void Init(float realWid, float realHei, float tileSize)
	{
		// init function separate from the contructor so that we can generate many perlin noise-es with the same object.
		this.realWid = realWid;
		this.realHei = realHei;
		this.tileSize = tileSize;
		this.wid = Mathf.FloorToInt(this.realWid/this.tileSize);
		this.hei = Mathf.FloorToInt(this.realHei/this.tileSize);
		CalculateNoise();
	}
	
	private void CalculateNoise()
	{
		noise = new float[this.wid,this.hei];
		int jj = 0;
		while (jj < this.hei)
		{
			int ii = 0;
			while (ii < this.wid)
			{
				noise[ii,jj] = Mathf.PerlinNoise(0.0f + ii*this.tileSize, 0.0f + jj*this.tileSize);
				ii++;
			}
			jj++;
		}
		RenderNoise();
	}
	
	public void RenderNoise()
	{
		//this.noiseTiles = new FSprite[this.wid,this.hei];
		for (int ii = 0; ii < this.wid; ii++)
		{
			for (int jj = 0; jj < this.hei; jj++)
			{
				var tile = new FSprite("whitetile");
				tile.color = new Color(noise[ii,jj],noise[ii,jj],noise[ii,jj]);
				tile.x = ii*this.tileSize;
				tile.y = jj*this.tileSize;
				this.map.stage.AddChild(tile);
				//this.noiseTiles[ii,jj] = new FSprite("whitetile");
			}
		}
	}
}