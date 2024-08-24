using System;
using System.Collections.Generic;
using System.Linq;

namespace QMud.Core
{
	/// <summary>
	/// Common superclass for players and monsters.
	/// </summary>
	public abstract class Living
	{
		public const int SearchRoom = (1 << 0);
		public const int SearchInv = (1 << 1);
		public const int SearchEq = (1 << 2);

		// Runtime attributes
		public Room InRoom { get; set; }
		public Party InParty { get; set; }
		public Fight InFight { get; set; }
		public FightActions NextAction { get; set; }

		// Items
		public List<Item> Inventory { get; private set; }
		public List<Item> Equipment { get; private set; }

		// Health
		public int Blood { get; set; }
		protected BodyPart[] Body { get; private set; }

		public Living ()
		{
			Inventory = new List<Item>();
			Equipment = new List<Item>();

			int partCount = Enum.GetNames(typeof(BodyParts)).Length;
			Body = new BodyPart[partCount];

			for (int i = 0; i < partCount; i++)
			{
				Body[i] = new BodyPart((BodyParts) i);	
			}
		}

		#region CanDoSomething functions
		public bool CanDrop(Item item)
		{
			// For now all items can be dropped
			return true;
		}

		public bool CanGet(Item item)
		{
			// For now all items can be picked up
			return true;
		}

		public bool CanRemove(Item item)
		{
			// For now all items can be removed
			return true;
		}

		/// <summary>
		/// Can this monster/player see other monster/player?
		/// </summary>
		/// <param name="other">
		/// Target of this characters sight. <see cref="QBaseObject"/>
		/// </param>
		/// <returns>
		/// True or false. <see cref="System.Boolean"/>
		/// </returns>
		public bool CanSee(Living other)
		{
			// We can always see ourself
			if (other == this)
			{
				return true;
			}

			if (IsBlind())
			{
				return false;
			}
				
			return true;
		}

		public bool CanSee(Item item)
		{
			if (IsBlind())
			{
				return false;
			}

			return true;
		}

		public bool CanWear(Item item)
		{
			// For now all items can be used by everyone
			return true;
		}
		#endregion

		#region Abstract functions
		public abstract bool ChooseNextAction ();
		public abstract void Die ();
		public abstract string GetName ();
		public abstract Genders GetSex ();
		#endregion

		public BodyPart GetBodyPart(BodyParts part)
		{
			return Body[(int)part];
		}

		public Item FindItem (string keyword, int targets)
		{
			List<Item> search_list = new List<Item>();

			// Search room
			if ((targets & SearchRoom) != 0)
			{
				search_list.AddRange(this.InRoom.Items);
			}

			// Search inventory
			if ((targets & SearchInv) != 0)
			{
				search_list.AddRange(Inventory);
			}

			// Search equipment
			if ((targets & SearchEq) != 0)
			{
				search_list.AddRange(Equipment);
			}

			return Find.FindItem(search_list, keyword, this);
		}

		public string HeShe ()
		{
			if (GetSex() == Genders.Male)
			{
				return "he";
			}
			else if (GetSex() == Genders.Female)
			{
				return "she";
			}
			else
			{
				return "it";
			}
		}

		public string HimHer ()
		{
			if (GetSex() == Genders.Male)
			{
				return "him";
			}
			else if (GetSex() == Genders.Female)
			{
				return "her";
			}
			else
			{
				return "it";
			}
		}

		public string HisHer ()
		{
			if (GetSex() == Genders.Male)
			{
				return "his";
			}
			else if (GetSex() == Genders.Female)
			{
				return "her";
			}
			else
			{
				return "its";
			}
		}

		public bool IsBlind ()
		{
			return GetBodyPart(BodyParts.LeftEye).IsDestroyed() && GetBodyPart(BodyParts.RightEye).IsDestroyed();
		}

		public bool IsInSameParty (Living other)
		{
			return this.InParty != null && this.InParty == other.InParty;
		}

		public bool IsMonster ()
		{
			return (this.GetType() == typeof(Monster));
		}

		public bool IsPlayer ()
		{
			return (this.GetType() == typeof(Player));
		}
	}
}
