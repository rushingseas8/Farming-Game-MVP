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

	public static Block lookup(int x, int z) {
		return blocks.Find (elem => elem.x == x && elem.z == z);
	}

	public static float heightLookup(int x, int z) {
		Block block = lookup (x, z);
		if (block == null) {
			return -1.0f;
		}
		return block.height;
	}

	public static float nutritionLookup(int x, int z) {
		Block block = lookup (x, z);
		if (block == null) {
			return -1.0f;
		}
		return block.nutrition;
	}
}
