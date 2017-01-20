﻿// Copyright 2014-2017 ClassicalSharp | Licensed under BSD-3
using System;
using OpenTK.Input;

namespace ClassicalSharp.Mode {
	
	public sealed class SurvivalGameMode : IGameMode {
		
		Game game;
		int score = 0;
		byte[] invCount = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 10 };
		Random rnd = new Random();
		
		public bool HandlesKeyDown(Key key) { return false; }

		public void PickLeft(byte old) {
			Vector3I pos = game.SelectedPos.BlockPos;
			game.UpdateBlock(pos.X, pos.Y, pos.Z, 0);
			game.UserEvents.RaiseBlockChanged(pos, old, 0);
			HandleDelete(old);
		}
		
		public void PickMiddle(byte old) {
		}
		
		public void PickRight(byte old, byte block) {
			int index = game.Inventory.HeldBlockIndex;
			if (invCount[index] == 0) return;
			
			Vector3I pos = game.SelectedPos.TranslatedPos;
			game.UpdateBlock(pos.X, pos.Y, pos.Z, block);
			game.UserEvents.RaiseBlockChanged(pos, old, block);
			
			invCount[index]--;
			if (invCount[index] != 0) return;
			
			// bypass HeldBlock's normal behaviour
			game.Inventory.Hotbar[index] = Block.Invalid;
			game.Events.RaiseHeldBlockChanged();
		}
		
		public bool PickEntity(byte id) {
			return false;
		}
		
		
		void HandleDelete(byte old) {
			if (old == Block.Log) {
				AddToHotbar(Block.Wood, rnd.Next(3, 6));
			} else if (old == Block.CoalOre) {
				AddToHotbar(Block.Slab, rnd.Next(1, 4));
			} else if (old == Block.IronOre) {
				AddToHotbar(Block.Iron, 1);
			} else if (old == Block.GoldOre) {
				AddToHotbar(Block.Gold, 1);
			} else if (old == Block.Grass) {
				AddToHotbar(Block.Dirt, 1);
			} else if (old == Block.Stone) {
				AddToHotbar(Block.Cobblestone, 1);
			} else if (old == Block.Leaves) {
				if (rnd.Next(1, 16) == 1) { // TODO: is this chance accurate?
					AddToHotbar(Block.Sapling, 1);
				}
			} else {
				AddToHotbar(old, 1);
			}
		}
		
		void AddToHotbar(byte block, int count) {
			int index = -1;
			byte[] hotbar = game.Inventory.Hotbar;
			
			// Try searching for same block, then try invalid block
			for (int i = 0; i < hotbar.Length; i++) {
				if (hotbar[i] == block) index = i;
			}
			if (index == -1) {
				for (int i = hotbar.Length - 1; i >= 0; i--) {
					if (hotbar[i] == Block.Invalid) index = i;
				}
			}
			if (index == -1) return; // no free slots
			
			for (int j = 0; j < count; j++) {
				if (invCount[index] >= 99) return; // no more count
				hotbar[index] = block;
				invCount[index]++; // TODO: do we need to raise an event if changing held block still?
				// TODO: we need to spawn block models instead
			}
		}

		
		public void OnNewMapLoaded(Game game) {
			game.Chat.Add("&fScore: &e" + score, MessageType.Status1);
		}
		
		public void Init(Game game) {
			this.game = game;
			byte[] hotbar = game.Inventory.Hotbar;
			for (int i = 0; i < hotbar.Length; i++)
				hotbar[i] = Block.Invalid;
			hotbar[hotbar.Length - 1] = Block.TNT;
		}
		
		
		public void Ready(Game game) { }
		public void Reset(Game game) { }
		public void OnNewMap(Game game) { }
		public void Dispose() { }
	}
}