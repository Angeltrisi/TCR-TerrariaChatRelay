﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Discord.WebSocket;
using Terraria;
using TerrariaChatRelay.Helpers;

namespace TerrariaChatRelay.Clients.DiscordClient.Helpers
{
	public class ChatParser
	{
		Regex specialFinder { get; }
		Regex userMentionFinder { get; }
		Regex colorCodeFinder { get; }
		Regex itemCodeFinder { get; }

		public ChatParser()
		{
			specialFinder = new Regex(@":[^:\s]*(?:::[^:\s]*)*>");
			userMentionFinder = new Regex(@"<@!*&*[0-9]+>");
			colorCodeFinder = new Regex(@"\[c\/.*?:(.*?)\]");
			itemCodeFinder = new Regex(@"\[i:(.*?)\]");
		}

		public string ConvertUserIdsToNames(string chatMessage, IReadOnlyCollection<SocketUser> users)
		{
			foreach (var user in users)
			{
				chatMessage = chatMessage.Replace($"<@{user.Id}>", $"[c/00FFFF:@" + user.Username.Replace("[", "").Replace("]", "") + "]");
				chatMessage = chatMessage.Replace($"<@!{user.Id}>", $"[c/00FFFF:@" + user.Username.Replace("[", "").Replace("]", "") + "]");
			}

			return chatMessage;
		}

		public string ShortenEmojisToName(string chatMessage)
		{
			chatMessage = specialFinder.Replace(chatMessage, ":");
			chatMessage = chatMessage.Replace("<:", ":");
			chatMessage = chatMessage.Replace("<a:", ":");

			return chatMessage;
		}

		public string RemoveUserMentions(string chatMessage)
		{
			chatMessage = userMentionFinder.Replace(chatMessage, "<User Ping Removed>");
			chatMessage = chatMessage.Replace("@everyone", "@-everyone");
			return chatMessage;

		}

		public string RemoveTerrariaColorAndItemCodes(string chatMessage)
		{
			var match = colorCodeFinder.Match(chatMessage);

			while (match.Success)
			{
				if (match.Groups.Count >= 2)
					chatMessage = chatMessage.Replace(match.Groups[0].Value, match.Groups[1].Value);

				match = match.NextMatch();
			}

			match = itemCodeFinder.Match(chatMessage);

			while (match.Success)
			{
				if (match.Groups.Count >= 2)
					chatMessage = chatMessage.Replace(match.Groups[0].Value, match.Groups[1].Value);

				match = match.NextMatch();
			}

			return chatMessage;
		}

		public string ReplaceCustomStringVariables(string input)
		{
            var players = Main.player.Where(x => x.name.Length != 0);

            input = input.Replace("%worldname%", Game.World.GetName())
                    .Replace("%playercount%", players.Count().ToString())
                    .Replace("%maxplayers%", Main.maxNetPlayers.ToString());

			return input;
        }
	}
}