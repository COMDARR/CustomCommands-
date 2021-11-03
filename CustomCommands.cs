using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using Microsoft.Xna.Framework;

namespace CustomCommands
{
	[ApiVersion(2, 1)]
	public class CustomCommands : TerrariaPlugin
	{

		

		public override string Author => "Comdar";


		public override string Description => "Some custom commands for TShock.";


		public override string Name => "CustomCommands";


		public override Version Version => new Version(1, 0, 0, 0);

		public string HelpText { get; private set; }
        public static object[] Specifier { get; private set; }

        public CustomCommands(Main game)
			: base(game)
		{

		}



		public override void Initialize()
		{
			Commands.ChatCommands.Remove(Commands.TShockCommands.Where(c => c.Name == "spawn").First());
			Commands.ChatCommands.Remove(Commands.TShockCommands.Where(c => c.Name == "home").First());
			Commands.ChatCommands.Add(new Command("spawn", spawncooldown, "spawn"));
			Commands.ChatCommands.Add(new Command("roll", Roll, "roll"));
			Commands.ChatCommands.Add(new Command("getpos", getpos, "getpos"));
			Commands.ChatCommands.Add(new Command("admin.setnick", setnick, "nick"));
			Commands.ChatCommands.Add(new Command("mod.broadcast", modbc, "modbc"));
			Commands.ChatCommands.Add(new Command("admin.broadcast", adminbc, "adminbc"));
			Commands.ChatCommands.Add(new Command("owner.broadcast", ownerbc, "ownerbc"));
		}


        Timer timer = new Timer(5000);
		Timer timerMessage = new Timer(1000);
		int cooldown = 5;

		void Roll(CommandArgs args)
		{
			Random r = new Random();
			TSPlayer.All.SendMessage(args.Player.Name + " rolled: " + r.Next(1, 20), Color.Orange);
		}
		void getpos(CommandArgs args)
		{
			int x = (int)args.Player.X / 16;
			int y = (int)args.Player.Y / 16;
			args.Player.SendMessage(string.Format("X: {0}, Y: {1}", x, y), Color.Lime);
		}
		void setnick(CommandArgs args)
		{
			if (args.Player == null) return;

			var plr = args.Player;
			if (args.Parameters.Count < 1)
			{
				args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /nick <newname>");
				return;
			}
			string newName = string.Join(" ", args.Parameters).Trim();

			#region Checks
			if (newName.Length < 2)
			{
				args.Player.SendMessage("A name must be at least 2 characters long.", Color.Pink);
				return;
			}

			if (newName.Length > 20)
			{
				args.Player.SendMessage("A name must not be longer than 20 characters.", Color.Pink);
				return;
			}

			List<TSPlayer> SameName = TShock.Players.Where(player => (player != null && player.Name == newName)).ToList();
			if (SameName.Count > 0)
			{
				args.Player.SendMessage("This name is taken by another player.", Color.Red);
				return;
			}
			#endregion Checks

			string oldName = plr.TPlayer.name;
			plr.TPlayer.name = newName;
			TShock.Utils.Broadcast(string.Format("{0} has changed his name to {1}.", oldName, newName), Color.Lime);
			plr.SendData(PacketTypes.PlayerInfo, newName, plr.Index);
		}
		void spawncooldown(CommandArgs args)
		{
			{
				if (timer.Enabled)
				{
					args.Player.SendMessage("Please don't spam this command", Color.Red);
				}
				else
				{
					cooldown = 5;
					timer = new Timer(5000);
					timerMessage = new Timer(1000);
					timerMessage.Elapsed += (sender, e) => Message(sender, e, args);
					timer.Elapsed += (sender, e) => Teleport(sender, e, args);
					timerMessage.Start();
					timer.Start();

					args.Player.SendMessage("5 seconds left before teleportation", Color.LightBlue);
				}
			}
		}

		void Message(object sender, ElapsedEventArgs e, CommandArgs args)
		{
			cooldown -= 1;
			if (cooldown == 1)
			{
				args.Player.SendMessage("" + cooldown + " seconds left before teleportation", Color.LightBlue);
			}
			else
			{
				args.Player.SendMessage("" + cooldown + " seconds left before teleportation", Color.LightBlue);
			}
		}

		void Teleport(object sender, ElapsedEventArgs e, CommandArgs args)
		{
			timerMessage.Stop();
			timer.Stop();
			if (args.Player.Teleport(Main.spawnTileX * 16, (Main.spawnTileY * 16) - 48))
			{
				args.Player.SendMessage("You have been teleported to spawn", Color.Lime);
			}
		}

		private void modbc(CommandArgs args)
		{
			string message = string.Join(" ", args.Parameters);

			TShock.Utils.Broadcast(
				"(Moderator broadcast) " + message,
				Convert.ToByte(TShock.Config.Settings.BroadcastRGB[0]), Convert.ToByte(TShock.Config.Settings.BroadcastRGB[1]),
				Convert.ToByte(TShock.Config.Settings.BroadcastRGB[2]));
		}

		private void adminbc(CommandArgs args)
		{
			string message = string.Join(" ", args.Parameters);

			TShock.Utils.Broadcast(
				"(Admin broadcast) " + message,
				Convert.ToByte(TShock.Config.Settings.BroadcastRGB[0]), Convert.ToByte(TShock.Config.Settings.BroadcastRGB[1]),
				Convert.ToByte(TShock.Config.Settings.BroadcastRGB[2]));
		}

		private void ownerbc(CommandArgs args)
		{
			string message = string.Join(" ", args.Parameters);

			TShock.Utils.Broadcast(
				"(Onwer broadcast) " + message,
				Convert.ToByte(TShock.Config.Settings.BroadcastRGB[0]), Convert.ToByte(TShock.Config.Settings.BroadcastRGB[1]),
				Convert.ToByte(TShock.Config.Settings.BroadcastRGB[2]));
		}


		void Teleport(CommandArgs args)
		{
			if (args.Player.Teleport(Main.spawnTileX * 16, (Main.spawnTileY * 16) - 48))
			{
				args.Player.SendMessage("You have been teleported to spawn", Color.Lime);
			}
		}
	}
}


