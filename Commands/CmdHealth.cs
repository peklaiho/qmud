using System;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdHealth : BaseCommand
	{
		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			string[] bodyPartNames =
			{
				"head",
				"right eye",
				"left eye",
				"neck",
				"chest",
				"back",
				"right arm",
				"left arm",
				"right hand",
				"left hand",
				"stomach",
				"right leg",
				"left leg",
				"right foot",
				"left foot"
			};

			for (int i = 0; i < bodyPartNames.Length; i++)
			{
				int health = player.GetBodyPart((BodyParts)i).Health;

				if (health <= 29)
				{
					player.Out(player.GetColor(Color.BrightRed));
				}
				else if (health <= 59)
				{
					player.Out(player.GetColor(Color.Red));
				}
				else if (health <= 89)
				{
					player.Out(player.GetColor(Color.Yellow));
				}
				else
				{
					player.Out(player.GetColor(Color.Green));
				}

				player.OutLn("Your " + bodyPartNames[i] + HealthToString(health, (BodyParts)i) + player.GetColor(Color.Reset));
			}
		}

		public override string GetHelpText ()
		{
			return "Display the health of your character or your target.";
		}

		private static string HealthToString(int health, BodyParts part)
		{
			if (health <= 0)
			{
				if (part == BodyParts.LeftEye || part == BodyParts.RightEye)
				{
					return " is blind.";
				}
				else if (part == BodyParts.LeftHand || part == BodyParts.RightHand ||
					part == BodyParts.LeftArm || part == BodyParts.RightArm ||
					part == BodyParts.LeftFoot || part == BodyParts.RightFoot ||
					part == BodyParts.LeftLeg || part == BodyParts.RightLeg)
				{
					return " has been severed from the body.";
				}
				else if (part == BodyParts.Head)
				{
					return " has been decapitated.";
				}
				else
				{
					return " has been hacked to pieces.";
				}
			}
			else if (health <= 29)
			{
				if (part == BodyParts.LeftHand || part == BodyParts.RightHand ||
					part == BodyParts.LeftArm || part == BodyParts.RightArm ||
					part == BodyParts.LeftFoot || part == BodyParts.RightFoot ||
					part == BodyParts.LeftLeg || part == BodyParts.RightLeg)
				{
					return " has been lethally damaged and is barely dangling on.";
				}
				else
				{
					return " has been lethally wounded and is gushing blood.";
				}
			}
			else if (health <= 59)
			{
				return " has been lethally wounded.";
			}
			else if (health <= 79)
			{
				return " has been severely wounded.";
			}
			else if (health <= 89)
			{
				return " has been mildly wounded.";
			}
			else if (health <= 99)
			{
				return " has a scratch.";
			}
			else
			{
				return " is in perfect condition.";
			}
		}
	}
}
