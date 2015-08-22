using System;
using System.IO;
using System.Collections.Generic;
using CreatureModule;
using MeshBoneUtil;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreatureSwitchItemPacket : MonoBehaviour {
	public String packet_name;
	public float posX, posY, item_width, item_height, canvas_width, canvas_height;
	
	#if UNITY_EDITOR
	[MenuItem("GameObject/Creature/CreatureSwitchItemPacket")]
	static CreatureSwitchItemPacket CreatePacket()
	{
		GameObject newObj = new GameObject();
		newObj.name = "New Creature Switch Item Packet";
		CreatureSwitchItemPacket new_packet;
		new_packet = newObj.AddComponent<CreatureSwitchItemPacket> () as CreatureSwitchItemPacket;
		
		return new_packet;
	}
	#endif
}