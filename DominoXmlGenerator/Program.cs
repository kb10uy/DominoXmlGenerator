using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;


namespace DominoXmlGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }
            string[] all = File.ReadAllLines(args[0]);
            new Parser().Run(all, (args.Length >= 2) ? args[1] : args[0] + ".xml");
        }
    }

    class Parser
    {
        XmlDocument doc = new XmlDocument();
        XmlElement root, inst, drum, ccm, tmpl, defl;
        Action<IReadOnlyList<string>> crootact;

        public void Run(IReadOnlyList<string> lines, string outname)
        {
            root = doc.CreateElement("ModuleData");
            doc.AppendChild(root);

            inst = doc.CreateElement("InstrumentList");
            drum = doc.CreateElement("DrumSetList");
            ccm = doc.CreateElement("ControlChangeMacroList");
            tmpl = doc.CreateElement("TemplateList");
            defl = doc.CreateElement("DefaultData");

            root.AppendChild(inst);
            root.AppendChild(drum);
            root.AppendChild(ccm);
            root.AppendChild(tmpl);
            root.AppendChild(defl);

            crootact = ProcessInstruments;
            foreach (var i in lines)
            {
                var raw = i.Split(new[] { ',' }, StringSplitOptions.None);
                switch (raw[0])
                {
                    case ".Name":
                        root.SetAttribute("Name", raw[1]);
                        continue;
                    case ".Folder":
                        root.SetAttribute("Folder", raw[1]);
                        continue;
                    case ".Priority":
                        root.SetAttribute("Priority", raw[1]);
                        continue;
                    case ".Creator":
                        root.SetAttribute("FileCreator", raw[1]);
                        continue;
                    case ".Version":
                        root.SetAttribute("FileVersion", raw[1]);
                        continue;
                    case ".Website":
                        root.SetAttribute("WebSite", raw[1]);
                        continue;

                    case "Instruments":
                        crootact = ProcessInstruments;
                        break;
                    case "Drumsets":
                        crootact = ProcessDrumsets;
                        break;
                    case "ControlChangeMacros":
                        crootact = ProcessControlChangeMacros;
                        break;
                    case "Templates":
                        crootact = ProcessTemplates;
                        break;
                    case "Defaults":
                        crootact = ProcessDefaults;
                        break;
                }
                crootact(raw.Skip(1).ToList());
            }
            //doc.Save(outname);
            using(var fs=new FileStream(outname,FileMode.OpenOrCreate,FileAccess.Write))
            using (var sw = new StreamWriter(fs, Encoding.GetEncoding(932)))
            {
                doc.Save(sw);
            }
        }

        XmlElement cim;
        private void ProcessInstruments(IReadOnlyList<string> list)
        {
            if (list[0] == "Map")
            {
                cim = doc.CreateElement("Map");
                inst.AppendChild(cim);
            }
            switch (list[1])
            {
                case ".Name":
                    cim.SetAttribute("Name", list[2]);
                    return;
                case "PC":
                    cimp = doc.CreateElement("PC");
                    cim.AppendChild(cimp);
                    break;
            }
            ProcessInstrumentsPc(list.Skip(2).ToList());
        }

        XmlElement cimp;
        private void ProcessInstrumentsPc(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Name":
                    cimp.SetAttribute("Name", list[1]);
                    return;
                case ".Number":
                    cimp.SetAttribute("PC", list[1]);
                    return;
                case "Bank":
                    cimpb = doc.CreateElement("Bank");
                    cimp.AppendChild(cimpb);
                    break;
            }
            ProcessInstrumentsPcBank(list.Skip(1).ToList());
        }

        XmlElement cimpb;
        private void ProcessInstrumentsPcBank(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Name":
                    cimpb.SetAttribute("Name", list[1]);
                    return;
                case ".LSB":
                    cimpb.SetAttribute("LSB", list[1]);
                    return;
                case ".MSB":
                    cimpb.SetAttribute("MSB", list[1]);
                    return;
            }
        }

        XmlElement cdm;
        private void ProcessDrumsets(IReadOnlyList<string> list)
        {
            if (list[0] == "Map")
            {
                cdm = doc.CreateElement("Map");
                drum.AppendChild(cdm);
            }
            switch (list[1])
            {
                case ".Name":
                    cdm.SetAttribute("Name", list[2]);
                    return;
                case "PC":
                    cdmp = doc.CreateElement("PC");
                    cdm.AppendChild(cdmp);
                    break;
            }
            ProcessDrumsetsPc(list.Skip(2).ToList());
        }

        XmlElement cdmp;
        private void ProcessDrumsetsPc(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Name":
                    cdmp.SetAttribute("Name", list[1]);
                    return;
                case ".Number":
                    cdmp.SetAttribute("PC", list[1]);
                    return;
                case "Bank":
                    cdmpb = doc.CreateElement("Bank");
                    cdmp.AppendChild(cdmpb);
                    break;
            }
            ProcessDrumsetsPcBank(list.Skip(1).ToList());
        }

        XmlElement cdmpb;
        private void ProcessDrumsetsPcBank(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Name":
                    cdmpb.SetAttribute("Name", list[1]);
                    return;
                case ".LSB":
                    cdmpb.SetAttribute("LSB", list[1]);
                    return;
                case ".MSB":
                    cdmpb.SetAttribute("MSB", list[1]);
                    return;
                case "Tone":
                    cdmpbt = doc.CreateElement("Tone");
                    cdmpb.AppendChild(cdmpbt);
                    break;
            }
            ProcessDrumsetsPcBankTone(list.Skip(1).ToList());
        }

        XmlElement cdmpbt;
        private void ProcessDrumsetsPcBankTone(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Name":
                    cdmpbt.SetAttribute("Name", list[1]);
                    return;
                case ".Key":
                    cdmpbt.SetAttribute("Key", list[1]);
                    return;
            }
        }

        Action<IReadOnlyList<string>> cca;
        int ccfl = 0;
        private void ProcessControlChangeMacros(IReadOnlyList<string> list)
        {
            ccfl = 0;
            switch (list[0])
            {
                case "Folder":
                    while (ccf.Count > 0) ccf.Pop();
                    cca = ProcessControlChangeMacrosFolder;
                    var f = doc.CreateElement("Folder");
                    ccm.AppendChild(f);
                    ccf.Push(f);
                    break;
                case "CCM":
                    cca = ProcessControlChangeMacrosCcm;
                    ccfc = doc.CreateElement("CCM");
                    ccm.AppendChild(ccfc);
                    break;
            }
            ccfl++;
            cca(list.Skip(1).ToList());
            //TODO: Link系とTableの対応
        }

        Action<IReadOnlyList<string>> ccfa;
        Stack<XmlElement> ccf = new Stack<XmlElement>();
        private void ProcessControlChangeMacrosFolder(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Name":
                    while (ccf.Count > ccfl) ccf.Pop();
                    ccf.Peek().SetAttribute("Name", list[1]);
                    return;
                case ".Id":
                    while (ccf.Count > ccfl) ccf.Pop();
                    ccf.Peek().SetAttribute("ID", list[1]);
                    return;
                case "CCM":
                    while (ccf.Count > ccfl) ccf.Pop();
                    ccfa = ProcessControlChangeMacrosCcm;
                    ccfc = doc.CreateElement("CCM");
                    ccf.Peek().AppendChild(ccfc);
                    break;
                case "CCMLink":
                    while (ccf.Count > ccfl) ccf.Pop();
                    ccfa = ProcessControlChangeMacrosCcmLink;
                    ccfcl = doc.CreateElement("CCMLink");
                    ccf.Peek().AppendChild(ccfcl);
                    break;
                case "FolderLink":
                    while (ccf.Count > ccfl) ccf.Pop();
                    ccfa = ProcessControlChangeMacrosFolderLink;
                    ccffl = doc.CreateElement("FolderLink");
                    ccf.Peek().AppendChild(ccffl);
                    break;
                case "Table":
                    ccfa = ProcessControlChangeMacrosTable;
                    ccfct = doc.CreateElement("Table");
                    ccf.Peek().AppendChild(ccfct);
                    break;
                case "Folder":
                    while (ccf.Count > ccfl) ccf.Pop();
                    ccfa = ProcessControlChangeMacrosFolder;
                    var f = doc.CreateElement("Folder");
                    ccf.Peek().AppendChild(f);
                    ccf.Push(f);
                    break;
            }
            ccfl++;
            ccfa(list.Skip(1).ToList());
        }

        XmlElement ccfct;
        private void ProcessControlChangeMacrosTable(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Id":
                    ccfct.SetAttribute("ID", list[1]);
                    return;
                case "Entry":
                    ccfcve = doc.CreateElement("Table");
                    ccfct.AppendChild(ccfcve);
                    break;
            }
            ProcessControlChangeMacrosCcmEntry(list.Skip(1).ToList());
        }

        XmlElement ccfcl;
        private void ProcessControlChangeMacrosCcmLink(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Id":
                    ccfcl.SetAttribute("ID", list[1]);
                    return;
                case ".Value":
                    ccfcl.SetAttribute("Value", list[1]);
                    return;
                case ".Gate":
                    ccfcl.SetAttribute("Gate", list[1]);
                    return;
            }
        }

        XmlElement ccffl;
        private void ProcessControlChangeMacrosFolderLink(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Name":
                    ccffl.SetAttribute("Name", list[1]);
                    return;
                case ".Id":
                    ccffl.SetAttribute("ID", list[1]);
                    return;
                case ".Value":
                    ccffl.SetAttribute("Value", list[1]);
                    return;
                case ".Gate":
                    ccffl.SetAttribute("Gate", list[1]);
                    return;
            }
        }

        XmlElement ccfc;
        Action<IReadOnlyList<string>> ccfca;
        private void ProcessControlChangeMacrosCcm(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Id":
                    ccfc.SetAttribute("ID", list[1]);
                    return;
                case ".Name":
                    ccfc.SetAttribute("Name", list[1]);
                    return;
                case ".Color":
                    ccfc.SetAttribute("Color", list[1]);
                    return;
                case ".Sync":
                    ccfc.SetAttribute("Sync", list[1]);
                    return;
                case ".MuteSync":
                    ccfc.SetAttribute("MuteSync", list[1]);
                    return;
                case ".Memo":
                    var m = doc.CreateElement("Memo");
                    m.InnerText = list[1];
                    ccfc.AppendChild(m);
                    return;
                case ".Data":
                    var d = doc.CreateElement("Data");
                    d.InnerText = list[1];
                    ccfc.AppendChild(d);
                    return;
                case "Value":
                    ccfca = ProcessControlChangeMacrosCcmValue;
                    ccfcv = doc.CreateElement("Value");
                    ccfc.AppendChild(ccfcv);
                    break;
                case "Gate":
                    ccfca = ProcessControlChangeMacrosCcmGate;
                    ccfcg = doc.CreateElement("Gate");
                    ccfc.AppendChild(ccfcg);
                    break;
            }
            ccfca(list.Skip(1).ToList());
        }

        XmlElement ccfcv;
        private void ProcessControlChangeMacrosCcmValue(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Default":
                    ccfcv.SetAttribute("Default", list[1]);
                    return;
                case ".Min":
                    ccfcv.SetAttribute("Min", list[1]);
                    return;
                case ".Max":
                    ccfcv.SetAttribute("Max", list[1]);
                    return;
                case ".Offset":
                    ccfcv.SetAttribute("Offset", list[1]);
                    return;
                case ".Name":
                    ccfcv.SetAttribute("Name", list[1]);
                    return;
                case ".Type":
                    ccfcv.SetAttribute("Type", list[1]);
                    return;
                case ".TableId":
                    ccfcv.SetAttribute("TableId", list[1]);
                    return;
                case "Entry":
                    ccfcve = doc.CreateElement("Entry");
                    ccfcv.AppendChild(ccfcve);
                    break;
            }
            ProcessControlChangeMacrosCcmEntry(list.Skip(1).ToList());
        }

        XmlElement ccfcve;
        private void ProcessControlChangeMacrosCcmEntry(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Label":
                    ccfcve.SetAttribute("Label", list[1]);
                    return;
                case ".Value":
                    ccfcve.SetAttribute("Value", list[1]);
                    return;
            }
        }

        XmlElement ccfcg;
        private void ProcessControlChangeMacrosCcmGate(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Default":
                    ccfcg.SetAttribute("Default", list[1]);
                    return;
                case ".Min":
                    ccfcg.SetAttribute("Min", list[1]);
                    return;
                case ".Max":
                    ccfcg.SetAttribute("Max", list[1]);
                    return;
                case ".Offset":
                    ccfcg.SetAttribute("Offset", list[1]);
                    return;
                case ".Name":
                    ccfcg.SetAttribute("Name", list[1]);
                    return;
                case ".Type":
                    ccfcg.SetAttribute("Type", list[1]);
                    return;
                case ".TableId":
                    ccfcg.SetAttribute("TableId", list[1]);
                    return;
                case "Entry":
                    ccfcve = doc.CreateElement("Entry");
                    ccfcg.AppendChild(ccfcve);
                    break;
            }
            ProcessControlChangeMacrosCcmEntry(list.Skip(1).ToList());
        }

        Action<IReadOnlyList<string>> cta;
        int ctfl = 0;
        private void ProcessTemplates(IReadOnlyList<string> list)
        {
            ctfl = 0;
            switch (list[0])
            {
                case "Folder":
                    while (ctf.Count > 0) ccf.Pop();
                    cta = ProcessTemplatesFolder;
                    var f = doc.CreateElement("Folder");
                    tmpl.AppendChild(f);
                    ctf.Push(f);
                    break;
                case "Template":
                    cta = ProcessTemplatesTemplate;
                    ctt = doc.CreateElement("Template");
                    tmpl.AppendChild(ctt);
                    break;
            }
            ctfl++;
            cta(list.Skip(1).ToList());
        }

        Action<IReadOnlyList<string>> ctfa;
        Stack<XmlElement> ctf = new Stack<XmlElement>();
        private void ProcessTemplatesFolder(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Name":
                    while (ctf.Count > ctfl) ctf.Pop();
                    ctf.Peek().SetAttribute("Name", list[1]);
                    return;
                case "Template":
                    while (ccf.Count > ctfl) ccf.Pop();
                    ctfa = ProcessTemplatesTemplate;
                    ctt = doc.CreateElement("Template");
                    ctf.Peek().AppendChild(ctt);
                    break;
                case "Folder":
                    while (ccf.Count > ctfl) ccf.Pop();
                    ctfa = ProcessTemplatesFolder;
                    var f = doc.CreateElement("Folder");
                    ctf.Peek().AppendChild(f);
                    ctf.Push(f);
                    break;
            }
            ctfl++;
            ctfa(list.Skip(1).ToList());
        }

        XmlElement ctt;
        Action<IReadOnlyList<string>> ctta;

        private void ProcessTemplatesTemplate(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Id":
                    ctt.SetAttribute("ID", list[1]);
                    return;
                case ".Name":
                    ctt.SetAttribute("Name", list[1]);
                    return;
                case "CC":
                    ctta = ProcessTemplatesTemplateCc;
                    cttc = doc.CreateElement("CC");
                    ctt.AppendChild(cttc);
                    break;
                case "PC":
                    ctta = ProcessTemplatesTemplatePc;
                    cttp = doc.CreateElement("PC");
                    ctt.AppendChild(cttp);
                    break;
                case ".Comment":
                    var c = doc.CreateElement("Comment");
                    c.SetAttribute("Text", list[1]);
                    ctt.AppendChild(c);
                    return;
                case ".Memo":
                    var m = doc.CreateElement("Memo");
                    m.InnerText = list[1];
                    ctt.AppendChild(m);
                    return;
            }
            ctta(list.Skip(1).ToList());
        }

        XmlElement cttc;
        private void ProcessTemplatesTemplateCc(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Id":
                    cttc.SetAttribute("ID", list[1]);
                    return;
                case ".Value":
                    cttc.SetAttribute("Value", list[1]);
                    return;
                case ".Gate":
                    cttc.SetAttribute("Gate", list[1]);
                    return;
            }
        }

        XmlElement cttp;
        private void ProcessTemplatesTemplatePc(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".PC":
                    cttp.SetAttribute("PC", list[1]);
                    return;
                case ".MSB":
                    cttp.SetAttribute("MSB", list[1]);
                    return;
                case ".LSB":
                    cttp.SetAttribute("LSB", list[1]);
                    return;
                case ".Mode":
                    cttp.SetAttribute("Mode", list[1]);
                    return;
            }
        }

        Action<IReadOnlyList<string>> cdea;
        private void ProcessDefaults(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case "Track":
                    cdea = ProcessDefaultsTrack;
                    cdet = doc.CreateElement("Track");
                    defl.AppendChild(cdet);
                    break;
            }
            cdea(list.Skip(1).ToList());
        }

        Action<IReadOnlyList<string>> cdeta;
        XmlElement cdet;
        private void ProcessDefaultsTrack(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Name":
                    cdet.SetAttribute("Name", list[1]);
                    return;
                case ".Channel":
                    cdet.SetAttribute("Ch", list[1]);
                    return;
                case ".Mode":
                    cdet.SetAttribute("Mode", list[1]);
                    return;
                case "Time":
                    cdeta = ProcessDefaultsTrackTime;
                    cdett = doc.CreateElement("TimeSignature");
                    cdet.AppendChild(cdett);
                    break;
                case "Key":
                    cdeta = ProcessDefaultsTrackKey;
                    cdetk = doc.CreateElement("KeySignature");
                    cdet.AppendChild(cdetk);
                    break;
                case "PC":
                    cdeta = ProcessDefaultsTrackPc;
                    cdetp = doc.CreateElement("PC");
                    cdet.AppendChild(cdetp);
                    break;
                case "CC":
                    cdeta = ProcessDefaultsTrackCc;
                    cdetc = doc.CreateElement("CC");
                    cdet.AppendChild(cdetc);
                    break;
                case "End":
                    cdeta = ProcessDefaultsTrackEnd;
                    cdete = doc.CreateElement("EOT");
                    cdet.AppendChild(cdete);
                    break;
                case "Tempo":
                    cdeta = ProcessDefaultsTrackTempo;
                    cdette = doc.CreateElement("Tempo");
                    cdet.AppendChild(cdette);
                    break;
                case "Mark":
                    cdeta = ProcessDefaultsTrackMark;
                    cdetm = doc.CreateElement("Mark");
                    cdet.AppendChild(cdetm);
                    break;
                case "Comment":
                    cdeta = ProcessDefaultsTrackComment;
                    cdetcm = doc.CreateElement("Comment");
                    cdet.AppendChild(cdetcm);
                    break;
                case "Template":
                    cdeta = ProcessDefaultsTrackTemplate;
                    cdettm = doc.CreateElement("Template");
                    cdet.AppendChild(cdettm);
                    break;
            }
            cdeta(list.Skip(1).ToList());
        }

        XmlElement cdetm;
        private void ProcessDefaultsTrackMark(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Name":
                    cdetm.SetAttribute("Name", list[1]);
                    return;
                case ".Tick":
                    cdetm.SetAttribute("Tick", list[1]);
                    return;
                case ".Step":
                    cdetm.SetAttribute("Step", list[1]);
                    return;
            }
        }

        XmlElement cdete;
        private void ProcessDefaultsTrackEnd(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Tick":
                    cdete.SetAttribute("Tick", list[1]);
                    return;
            }
        }

        XmlElement cdette;
        private void ProcessDefaultsTrackTempo(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Tempo":
                    cdette.SetAttribute("Tempo", list[1]);
                    return;
                case ".Tick":
                    cdette.SetAttribute("Tick", list[1]);
                    return;
                case ".Step":
                    cdette.SetAttribute("Step", list[1]);
                    return;
            }
        }

        XmlElement cdett;
        private void ProcessDefaultsTrackTime(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Signature":
                    cdett.SetAttribute("TimeSignature", list[1]);
                    return;
                case ".Tick":
                    cdett.SetAttribute("Tick", list[1]);
                    return;
                case ".Step":
                    cdett.SetAttribute("Step", list[1]);
                    return;
            }
        }

        XmlElement cdetk;
        private void ProcessDefaultsTrackKey(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Signature":
                    cdetk.SetAttribute("KeySignature", list[1]);
                    return;
                case ".Tick":
                    cdetk.SetAttribute("Tick", list[1]);
                    return;
                case ".Step":
                    cdetk.SetAttribute("Step", list[1]);
                    return;
            }
        }

        XmlElement cdetc;
        private void ProcessDefaultsTrackCc(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Id":
                    cdetc.SetAttribute("ID", list[1]);
                    return;
                case ".Value":
                    cdetc.SetAttribute("Value", list[1]);
                    return;
                case ".Gate":
                    cdetc.SetAttribute("Gate", list[1]);
                    return;
                case ".Tick":
                    cdetc.SetAttribute("Tick", list[1]);
                    return;
                case ".Step":
                    cdetc.SetAttribute("Step", list[1]);
                    return;
            }
        }

        XmlElement cdetp;
        private void ProcessDefaultsTrackPc(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Number":
                    cdetp.SetAttribute("PC", list[1]);
                    return;
                case ".MSB":
                    cdetp.SetAttribute("MSB", list[1]);
                    return;
                case ".LSB":
                    cdetp.SetAttribute("LSB", list[1]);
                    return;
                case ".Mode":
                    cdetp.SetAttribute("Mode", list[1]);
                    return;
                case ".Tick":
                    cdetp.SetAttribute("Tick", list[1]);
                    return;
                case ".Step":
                    cdetp.SetAttribute("Step", list[1]);
                    return;
            }
        }

        XmlElement cdetcm;
        private void ProcessDefaultsTrackComment(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Text":
                    cdetcm.SetAttribute("Text", list[1]);
                    return;
                case ".Tick":
                    cdetcm.SetAttribute("Tick", list[1]);
                    return;
                case ".Step":
                    cdetcm.SetAttribute("Step", list[1]);
                    return;
            }
        }

        XmlElement cdettm;
        private void ProcessDefaultsTrackTemplate(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Id":
                    cdettm.SetAttribute("ID", list[1]);
                    return;
                case ".Tick":
                    cdettm.SetAttribute("Tick", list[1]);
                    return;
                case ".Step":
                    cdettm.SetAttribute("Step", list[1]);
                    return;
            }
        }
    }
}
