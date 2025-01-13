using Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient;

internal class PlayerItem
{
	public int Index { get; private set; }
	public string Name { get; private set; }
	public int ItemIndex { get; private set; }

	public PlayerItem(Player player, int itemIndex)
	{
		Index = player.Index;
		Name = player.Name;
		ItemIndex = itemIndex;
	}
}
