using Modkit.Mutagen;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

namespace Wave3Gen;

/// <summary>
/// Generates the plugin (ESL-flagged), its SEQ file and the silent voice files. Shared by the third-wave mods;
/// everything mod-specific lives in Mod.cs (ModInfo).
/// Usage: generator --out &lt;dir&gt; --ids &lt;formids.json&gt; --papyrus &lt;src/papyrus&gt; [--items &lt;vanilla-items.json&gt;] [--report &lt;file&gt;]
/// </summary>
internal static class Program
{
    private static int Main(string[] args)
    {
        string Arg(string name, string fallback)
        {
            var i = Array.IndexOf(args, name);
            return i >= 0 && i + 1 < args.Length ? args[i + 1] : fallback;
        }
        var outDir = Arg("--out", "build/plugin");
        var idsPath = Arg("--ids", "src/plugin/formids.json");
        var papyrusDir = Arg("--papyrus", "src/papyrus");
        var itemsPath = Arg("--items", "src/plugin/vanilla-items.json");
        var parent = Path.GetDirectoryName(Path.GetFullPath(outDir))!;
        var reportPath = Arg("--report", Path.Combine(parent, "plugin-report.json"));
        var voiceListPath = Arg("--voice-list", Path.Combine(parent, "voice-files.tsv"));
        if (Directory.Exists(outDir)) Directory.Delete(outDir, true);
        Directory.CreateDirectory(outDir);

        var modKey = ModKey.FromFileName(ModInfo.PluginName);
        var ids = new FormIdRegistry(idsPath, modKey);
        var mod = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);
        var kit = new Kit(mod, ids, ItemData.Load(itemsPath));
        ModInfo.Build(kit);

        mod.ModHeader.Flags |= SkyrimModHeader.HeaderFlag.Small; // ESL-flagged ESP
        mod.ModHeader.Stats.Version = 1.7f;
        mod.ModHeader.Author = "synthalorian";
        mod.ModHeader.Description = ModInfo.Description;

        var outPath = Path.Combine(outDir, ModInfo.PluginName);
        mod.BeginWrite.ToPath(outPath).WithNoLoadOrder().Write();
        ids.Save();
        foreach (var r in ids.Retired) Console.WriteLine($"[generate] note: retired EditorID kept reserved: {r}");
        Console.WriteLine($"[generate] wrote {outPath}");

        int seq;
        using (var written = SkyrimMod.CreateFromBinaryOverlay(outPath, SkyrimRelease.SkyrimSE)) seq = SeqFile.Write(outDir, written);
        Console.WriteLine($"[generate] SEQ: {seq} start-game-enabled quest(s)");

        var lines = SilentVoice.Plan(mod, t => kit.Speakers[t.FormKey]);
        SilentVoice.Write(outDir, lines);
        File.WriteAllLines(voiceListPath, new[] { "file\tvoice_type\tspeaker\tseconds\ttext" }
            .Concat(lines.Select(l => $"{l.RelativePath}\t{l.VoiceType}\t{l.Speaker}\t{l.Seconds:0.0}\t{l.Text}")));
        Console.WriteLine($"[generate] {lines.Count} silent voice files ({lines.Sum(l => l.Seconds):0} s), list in {voiceListPath}");

        var errors = PluginValidator.Validate(outPath, new PapyrusIndex(papyrusDir), new HashSet<ModKey> { Vanilla.Skyrim }, reportPath, kit.ExpectedUnfilled);
        return errors == 0 ? 0 : 1;
    }
}
