using System;

namespace QMud.Core
{
	public class Monster : Living
	{
		public MonsterTemplate Template { get; private set; }

		public Monster (MonsterTemplate newTemplate)
			: base()
		{
			Template = newTemplate;
		}

		public override bool ChooseNextAction ()
		{
			if (NextAction == FightActions.None)
			{
				// For now monsters just automatically attack
				NextAction = FightActions.Hit;
			}

			return true;
		}

		public override void Die ()
		{
			Handler.ExtractLiving(this);
		}

		public override string GetName ()
		{
			return Template.Name;
		}

		public override Genders GetSex ()
		{
			return Template.Sex;
		}
	}
}
