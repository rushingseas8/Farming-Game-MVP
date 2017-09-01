using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block {
	public static List<Block> blocks = new List<Block>();

	public int x, z;
	public float height;
	public float nutrition;

	public Block(int x, float y, int z, float nutrition) {
		this.x = x;
		this.height = y;
		this.z = z;
		this.nutrition = nutrition;
	}

	public Block lookup(int x, int z) {
		
	}
}
