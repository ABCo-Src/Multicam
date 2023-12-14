using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages.Data;
using ABCo.Multicam.Server.Features.Switchers.Core.OBS.Messages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ABCo.Multicam.Server.Features.Switchers.Core.OBS.Communication
{
	public static class OBSJSONConverterHelpers
	{
		public static void ReadAndGetSpanOfStartAndEnd(ref Utf8JsonReader reader, ref ReadOnlySpan<byte> target)
		{
			// Get a span to the very start of the data
			var start = reader.ValueSpan;
			int startPos = checked((int)reader.TokenStartIndex);

			// Skip to the end
			reader.Skip();

			// Now extend that span to reach across everything we skipped
			int endPos = checked((int)reader.TokenStartIndex) + 1;
			target = MemoryMarshal.CreateSpan(ref MemoryMarshal.GetReference(start), endPos - startPos);

			// Read past the end
			reader.Read();
		}

		public static string? ReadString(ref Utf8JsonReader reader)
		{
			string? str = reader.GetString();
			reader.Read();
			return str;
		}
	}
}
