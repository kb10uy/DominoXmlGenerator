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
            doc.Save(outname);
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
                    cdmpb.SetAttribute("Name", list[1]);
                    return;
                case ".Key":
                    cdmpb.SetAttribute("Key", list[1]);
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
            ProcessControlChangeMacrosCcmValueEntry(list.Skip(1).ToList());
        }

        XmlElement ccfcve;
        private void ProcessControlChangeMacrosCcmValueEntry(IReadOnlyList<string> list)
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
                    ccfcge = doc.CreateElement("Entry");
                    ccfcg.AppendChild(ccfcge);
                    break;
            }
            ProcessControlChangeMacrosCcmGateEntry(list.Skip(1).ToList());
        }

        XmlElement ccfcge;
        private void ProcessControlChangeMacrosCcmGateEntry(IReadOnlyList<string> list)
        {
            switch (list[0])
            {
                case ".Label":
                    ccfcge.SetAttribute("Label", list[1]);
                    return;
                case ".Value":
                    ccfcge.SetAttribute("Value", list[1]);
                    return;
            }
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
            //TODO: Link系とTableの対応
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
                    cttp.SetAttribute("ID", list[1]);
                    return;
                case ".MSB":
                    cttp.SetAttribute("Value", list[1]);
                    return;
                case ".LSB":
                    cttp.SetAttribute("Gate", list[1]);
                    return;
                case ".Mode":
                    cttp.SetAttribute("Gate", list[1]);
                    return;
            }
        }

        private void ProcessDefaults(IReadOnlyList<string> list)
        {

        }
    }
}
