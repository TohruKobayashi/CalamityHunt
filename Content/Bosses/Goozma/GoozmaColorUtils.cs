using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityHunt.Content.Bosses.Goozma
{
    public static class GoozmaColorUtils
    {
        public static Vector3[] Oil = new Vector3[] //regular
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(51, 46, 78).ToVector3(),
            new Color(113, 53, 146).ToVector3(),
            new Color(174, 23, 189).ToVector3(),
            new Color(237, 128, 60).ToVector3(),
            new Color(247, 255, 101).ToVector3(),
            new Color(176, 234, 85).ToVector3(),
            new Color(102, 219, 249).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Nuclear = new Vector3[] //nuclear throne
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(22, 81, 12).ToVector3(),
            new Color(87, 87, 87).ToVector3(),
            new Color(0, 236, 74).ToVector3(),
            new Color(66, 255, 176).ToVector3(),
            new Color(255, 255, 20).ToVector3(),
            new Color(186, 255, 0).ToVector3(),
            new Color(55, 255, 28).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };        
        
        public static Vector3[] Gold = new Vector3[] //obvious
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(23, 16, 9).ToVector3(),
            new Color(40, 28, 15).ToVector3(),
            new Color(213, 150, 78).ToVector3(),
            new Color(242, 192, 100).ToVector3(),
            new Color(255, 255, 147).ToVector3(),
            new Color(143, 99, 52).ToVector3(),
            new Color(111, 80, 41).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Grayscale = new Vector3[] //obvious 
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(51, 51, 51).ToVector3(),
            new Color(87, 87, 87).ToVector3(),
            new Color(120, 120, 120).ToVector3(),
            new Color(153, 153, 153).ToVector3(),
            new Color(235, 235, 235).ToVector3(),
            new Color(200, 200, 200).ToVector3(),
            new Color(187, 187, 187).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };
                
        public static Vector3[] Honey = new Vector3[] //honey; not the bees
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(51, 4, 19).ToVector3(),
            new Color(135, 58, 6).ToVector3(),
            new Color(154, 255, 24).ToVector3(),
            new Color(216, 93, 10).ToVector3(),
            new Color(216, 163, 24).ToVector3(),
            new Color(150, 150, 24).ToVector3(),
            new Color(190, 85, 143).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };        
        
        public static Vector3[] Masterful = new Vector3[] //master mode rarity; for the worthy
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(200, 20, 10).ToVector3(),
            new Color(235, 80, 20).ToVector3(),
            new Color(255, 100, 0).ToVector3(),
            new Color(255, 150, 0).ToVector3(),
            new Color(255, 100, 0).ToVector3(),
            new Color(235, 80, 0).ToVector3(),
            new Color(220, 20, 10).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] DarkEnergy = new Vector3[] //dark bowser 
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(49, 82, 156).ToVector3(),
            new Color(65, 98, 189).ToVector3(),
            new Color(98, 123, 197).ToVector3(),
            new Color(189, 164, 246).ToVector3(),
            new Color(148, 106, 246).ToVector3(),
            new Color(115, 65, 222).ToVector3(),
            new Color(64, 24, 180).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Frigid = new Vector3[] //white and blue; formerly squid girl
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(43, 109, 196).ToVector3(),
            new Color(100, 204, 239).ToVector3(),
            new Color(141, 229, 239).ToVector3(),
            new Color(239, 255, 253).ToVector3(),
            new Color(255, 255, 255).ToVector3(),
            new Color(224, 255, 236).ToVector3(),
            new Color(59, 186, 249).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Tritanopic = new Vector3[] //tritanopia 
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(113, 33, 140).ToVector3(),
            new Color(244, 85, 41).ToVector3(),
            new Color(244, 34, 41).ToVector3(),
            new Color(244, 85, 90).ToVector3(),
            new Color(213, 244, 255).ToVector3(),
            new Color(71, 255, 255).ToVector3(),
            new Color(73, 209, 255).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Caliginous = new Vector3[] //idk who this is
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(0, 0, 0).ToVector3(),
            new Color(54, 16, 54).ToVector3(),
            new Color(99, 22, 130).ToVector3(),
            new Color(135, 102, 143).ToVector3(),
            new Color(164, 169, 163).ToVector3(),
            new Color(236, 62, 2).ToVector3(),
            new Color(165, 42, 0).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Spectrum = new Vector3[] //zx spectrum console
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(0, 0, 255).ToVector3(),
            new Color(255, 0, 0).ToVector3(),
            new Color(255, 0, 255).ToVector3(),
            new Color(0, 255, 0).ToVector3(),
            new Color(255, 255, 255).ToVector3(),
            new Color(255, 255, 0).ToVector3(),
            new Color(0, 255, 255).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Evil = new Vector3[] //red and purple; maybe touch this one up
        {
            new Color(251, 233, 254).ToVector3(),
            new Color(210, 168, 249).ToVector3(),
            new Color(156, 139, 219).ToVector3(),
            new Color(156, 93, 190).ToVector3(),
            new Color(106, 66, 150).ToVector3(),
            new Color(103, 48, 62).ToVector3(),
            new Color(335, 87, 48).ToVector3(),
            new Color(38, 30, 50).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Shadowflame = new Vector3[] //shadowflame
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(69, 41, 150).ToVector3(),
            new Color(90, 24, 240).ToVector3(),
            new Color(181, 25, 248).ToVector3(),
            new Color(182, 141, 255).ToVector3(),
            new Color(251, 202, 255).ToVector3(),
            new Color(72, 102, 218).ToVector3(),
            new Color(69, 41, 150).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Dogmatic = new Vector3[] //dogma static
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(255, 255, 255).ToVector3(),
            new Color(0, 0, 0).ToVector3(),
            new Color(255, 255, 255).ToVector3(),
            new Color(0, 0, 0).ToVector3(),
            new Color(255, 255, 255).ToVector3(),
            new Color(0, 0, 0).ToVector3(),
            new Color(255, 255, 255).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] FIREBLU = new Vector3[] //FIREBLU
        {
            new Color(40, 0, 30).ToVector3(),
            new Color(71, 16, 10).ToVector3(),
            new Color(143, 53, 46).ToVector3(),
            new Color(194, 23, 89).ToVector3(),
            new Color(255, 38, 30).ToVector3(),
            new Color(15, 05, 01).ToVector3(),
            new Color(154, 164, 235).ToVector3(),
            new Color(22, 55, 255).ToVector3(),
            new Color(0, 0, 26).ToVector3()
        };

        public static Vector3[] Distorted = new Vector3[] //noxus palette (5/27/2023)
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(91, 69, 143).ToVector3(),
            new Color(123, 75, 230).ToVector3(),
            new Color(181, 91, 230).ToVector3(),
            new Color(110, 153, 255).ToVector3(),
            new Color(114, 218, 249).ToVector3(),
            new Color(110, 153, 255).ToVector3(),
            new Color(123, 75, 230).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Electric = new Vector3[] //blue and yellow
        {
            new Color(40, 0, 30).ToVector3(),
            new Color(255, 255, 100).ToVector3(),
            new Color(12, 12, 0).ToVector3(),
            new Color(355, 355, 289).ToVector3(),
            new Color(255, 238, 130).ToVector3(),
            new Color(15, 05, 01).ToVector3(),
            new Color(254, 234, 235).ToVector3(),
            new Color(126, 125, 25).ToVector3(),
            new Color(33, 30, 226).ToVector3()
        };

        public static Vector3[] Polterplasmic = new Vector3[] //polterghart
        {
            new Color(10, 10, 10).ToVector3(),
            new Color(15, 15, 100).ToVector3(),
            new Color(12, 12, 0).ToVector3(),
            new Color(13, 13, 13).ToVector3(),
            new Color(105, 105, 113).ToVector3(),
            new Color(122, 122, 144).ToVector3(),
            new Color(155, 255, 255).ToVector3(),
            new Color(133, 15, 25).ToVector3(),
            new Color(13, 13, 13).ToVector3()
        };

        public static Vector3[] Exhumed = new Vector3[] //red purple something?? idk
        {
            new Color(10, 10, 10).ToVector3(),
            new Color(115, 15, 140).ToVector3(),
            new Color(12, 12, 0).ToVector3(),
            new Color(142, 13, 13).ToVector3(),
            new Color(105, 105, 113).ToVector3(),
            new Color(122, 122, 144).ToVector3(),
            new Color(255, 155, 255).ToVector3(),
            new Color(113, 15, 145).ToVector3(),
            new Color(13, 13, 13).ToVector3()
        };

        public static Vector3[] Raptured = new Vector3[] //glitchy
        {
            new Vector3(-100f),
            new Vector3(0f),
            new Vector3(-0.2f, 0.1f, 0.4f),
            new Vector3(0f),
            new Vector3(1f, 0f, 0f),
            new Vector3(1f, 0.2f, 0.4f),
            new Vector3(0f),
            new Vector3(0.2f, 0.8f, 0.9f),
            new Vector3(1f, 1f, 0.4f)
        };

        public static Vector3[] Ibanical = new Vector3[] //blue and red again for the third time
        {
            new Vector3(0f),
            new Vector3(0f),
            new Vector3(-0.2f, 0.1f, 0.4f),
            new Vector3(0f),
            new Vector3(1f, 0f, 0f),
            new Vector3(1f, 0.2f, 0.4f),
            new Vector3(0f),
            new Vector3(0.2f, 0.8f, 0.9f),
            new Vector3(0f, 1f, 1f)
        };

        public static Vector3[] AcidRainbow = new Vector3[] //acidic rainbow
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(11, 0, 0).ToVector3(),
            new Color(15, 255, 255).ToVector3(),
            new Color(40, 0, 250).ToVector3(),
            new Color(255, 0, 250).ToVector3(),
            new Color(255, 20, 10).ToVector3(),
            new Color(244, 244, 0).ToVector3(),
            new Color(10, 256, 0).ToVector3(),
            new Color(15, 0, 0).ToVector3()
        };

        public static Vector3[] Rubicon = new Vector3[] //ruby con
        {
            new Color(255, 0, 0).ToVector3(),
            new Color(215, 145, 100).ToVector3(),
            new Color(55, 0, 0).ToVector3(),
            new Color(142, 13, 13).ToVector3(),
            new Color(200, 0, 0).ToVector3(),
            new Color(255, 50, 80).ToVector3(),
            new Color(155, 20, 50).ToVector3(),
            new Color(155, 0, 0).ToVector3(),
            new Color(55, 0, 0).ToVector3()
        };

        public static Vector3[] DoubleRainbow = new Vector3[] //colorshift
        {
            new Color(0, 0, 0).ToVector3(),
            Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.8f + 0.01f) % 1f, 0.4f, 0.4f).ToVector3(),
            Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.8f + 0.1f) % 1f, 0.6f, 0.5f).ToVector3(),
            Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.8f + 0.2f) % 1f, 0.9f, 0.6f).ToVector3(),
            Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.8f + 0.5f) % 1f, 1f, 0.7f).ToVector3(),
            Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.8f + 0.6f) % 1f, 0.9f, 0.6f).ToVector3(),
            Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.8f + 0.7f) % 1f, 0.6f, 0.5f).ToVector3(),
            Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.8f + 0.9f) % 1f, 0.4f, 0.4f).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Unpleasant => new Vector3[] //this unpleasant goozma shows up at your front door
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(158, 83, 0).ToVector3(),
            new Color(181, 75, 59).ToVector3(),
            new Color(210, 63, 108).ToVector3(),
            new Color(245, 58, 253).ToVector3(),
            new Color(185, 106, 195).ToVector3(),
            new Color(132, 144, 144).ToVector3(),
            new Color(38, 211, 41).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] OceanJasper => new Vector3[] //ocean jasper; willowmaine
        {
            new Color(240, 111, 60).ToVector3(),
            new Color(196, 52, 0).ToVector3(),
            new Color(240, 111, 60).ToVector3(),
            new Color(250, 193, 132).ToVector3(),
            new Color(255, 255, 212).ToVector3(),
            new Color(134, 166, 153).ToVector3(),
            new Color(46, 83, 75).ToVector3(),
            new Color(12, 141, 31).ToVector3(),
            new Color(46, 83, 75).ToVector3()
        };

        public static Vector3[] Seashell => new Vector3[] //seashell; delly
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(255, 170, 170).ToVector3(),
            new Color(255, 85, 85).ToVector3(),
            new Color(170, 85, 85).ToVector3(),
            new Color(85, 85, 85).ToVector3(),
            new Color(0, 85, 85).ToVector3(),
            new Color(0, 0, 8).ToVector3(),
            new Color(255, 85, 0).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] JackOLantern => new Vector3[] //jack o lantern; delly
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(170, 170, 0).ToVector3(),
            new Color(255, 255, 0).ToVector3(),
            new Color(255, 170, 0).ToVector3(),
            new Color(255, 85, 0).ToVector3(),
            new Color(170, 85, 0).ToVector3(),
            new Color(85, 85, 0).ToVector3(),
            new Color(0, 85, 0).ToVector3(),
            new Color(0, 0, 0).ToVector3()
            };

        public static Vector3[] CottonCandy => new Vector3[] //cotton candy; delly
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(0, 0, 170).ToVector3(),
            new Color(255, 170, 255).ToVector3(),
            new Color(170, 170, 255).ToVector3(),
            new Color(255, 85, 255).ToVector3(),
            new Color(170, 255, 255).ToVector3(),
            new Color(0, 170, 255).ToVector3(),
            new Color(0, 0, 255).ToVector3(),
            new Color(0, 0, 0).ToVector3()
            };

        public static Vector3[] MeanGreen => new Vector3[] //we love u green isaac; spoop
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(24, 64, 39).ToVector3(),
            new Color(57, 102, 73).ToVector3(),
            new Color(85, 121, 91).ToVector3(),
            new Color(110, 148, 116).ToVector3(),
            new Color(201, 234, 206).ToVector3(),
            new Color(162, 196, 168).ToVector3(),
            new Color(129, 169, 136).ToVector3(),
            new Color(0, 0, 0).ToVector3()
            };

        public static Vector3[] ColorCalibration => new Vector3[] //color tv calibration thing; moonbee
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(242, 242, 242).ToVector3(),
            new Color(255, 255, 0).ToVector3(),
            new Color(0, 255, 255).ToVector3(),
            new Color(0, 255, 0).ToVector3(),
            new Color(255, 0, 255).ToVector3(),
            new Color(255, 0, 0).ToVector3(),
            new Color(0, 0, 0).ToVector3(), //lol this ones placeholder, replace if he chooses a new color
            new Color(0, 0, 0).ToVector3()
            };

        public static Vector3[] Enchanted => new Vector3[] //aerialite enchanted palette; purified
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(60, 135, 25).ToVector3(),
            new Color(60, 178, 255).ToVector3(),
            new Color(60, 255, 255).ToVector3(),
            new Color(255, 255, 255).ToVector3(),
            new Color(60, 255, 255).ToVector3(),
            new Color(60, 178, 255).ToVector3(),
            new Color(60, 135, 25).ToVector3(),
            new Color(0, 0, 0).ToVector3()
            };

        public static Vector3[] Sisyphan => new Vector3[] //sanded enemies WILL NOT HEAL; spoop
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(79, 52, 25).ToVector3(),
            new Color(96, 67, 35).ToVector3(),
            new Color(136, 107, 63).ToVector3(),
            new Color(174, 145, 101).ToVector3(),
            new Color(217, 189, 142).ToVector3(),
            new Color(208, 180, 133).ToVector3(),
            new Color(190, 162, 115).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Babil => new Vector3[] //babil; willowmaine
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(251, 255, 250).ToVector3(),
            new Color(114, 219, 151).ToVector3(),
            new Color(114, 219, 151).ToVector3(),
            new Color(72, 153, 125).ToVector3(),
            new Color(48, 102, 84).ToVector3(),
            new Color(72, 153, 125).ToVector3(),
            new Color(114, 219, 151).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Doomsday => new Vector3[] //babil; purified
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(39, 0, 0).ToVector3(),
            new Color(83, 205, 205).ToVector3(),
            new Color(81, 1, 1).ToVector3(),
            new Color(165, 0, 0).ToVector3(),
            new Color(255, 0, 0).ToVector3(),
            new Color(165, 0, 0).ToVector3(),
            new Color(81, 1, 1).ToVector3(),
            new Color(0, 0, 0).ToVector3()
         };

        public static Vector3[] Rugamarian => new Vector3[] //la ruga; purified
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(24, 24, 24).ToVector3(),
            new Color(37, 37, 37).ToVector3(),
            new Color(82, 68, 59).ToVector3(),
            new Color(128, 128, 128).ToVector3(),
            new Color(138, 111, 97).ToVector3(),
            new Color(82, 68, 59).ToVector3(),
            new Color(24, 24, 24).ToVector3(),
            new Color(0, 0, 0).ToVector3()
         };

        public static Vector3[] Poozma => new Vector3[] //goozma if he lived in the sewers; delly
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(45, 24, 22).ToVector3(),
            new Color(74, 42, 25).ToVector3(),
            new Color(57, 50, 24).ToVector3(),
            new Color(72, 68, 53).ToVector3(),
            new Color(101, 80, 53).ToVector3(),
            new Color(110, 60, 45).ToVector3(),
            new Color(74, 38, 25).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Festive => new Vector3[] //here to spread a little christmas cheer; delly
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(150, 17, 8).ToVector3(),
            new Color(104, 164, 6).ToVector3(),
            new Color(144, 202, 118).ToVector3(),
            new Color(191, 16, 4).ToVector3(),
            new Color(150, 17, 8).ToVector3(),
            new Color(44, 118, 0).ToVector3(),
            new Color(40, 76, 2).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] MoonCarpet => new Vector3[] //Yeah make a pallette of my bathroom carpet -Moonburn; delly
{
            new Color(0, 0, 0).ToVector3(),
            new Color(156, 152, 143).ToVector3(),
            new Color(41, 45, 70).ToVector3(),
            new Color(251, 227, 179).ToVector3(),
            new Color(115, 106, 111).ToVector3(),
            new Color(164, 144, 120).ToVector3(),
            new Color(251, 227, 179).ToVector3(),
            new Color(41, 45, 70).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Hein => new Vector3[] //hein; willowmaine
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(247, 247, 247).ToVector3(),
            new Color(160, 160, 160).ToVector3(),
            new Color(240, 184, 64).ToVector3(),
            new Color(224, 104, 56).ToVector3(),
            new Color(184, 48, 32).ToVector3(),
            new Color(120, 40, 224).ToVector3(),
              new Color(64, 24, 112).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Zeromus => new Vector3[] //zeromus; willowmaine
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(205, 205, 205).ToVector3(),
            new Color(83, 205, 205).ToVector3(),
            new Color(4, 26, 204).ToVector3(),
            new Color(204, 175, 153).ToVector3(),
            new Color(201, 157, 129).ToVector3(),
            new Color(205, 95, 2).ToVector3(),
            new Color(205, 30, 23).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Water => new Vector3[] //ohmy god its the water user!!!; moonbee
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(136, 182, 231).ToVector3(),
            new Color(97, 168, 207).ToVector3(),
            new Color(72, 147, 206).ToVector3(),
            new Color(49, 131, 217).ToVector3(),
            new Color(27, 109, 200).ToVector3(),
            new Color(152, 231, 254).ToVector3(),
            new Color(255, 255, 255).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Stellar => new Vector3[] //stellar slime; moonbee
        {
            new Color(0, 0, 0).ToVector3(),
             new Color(115, 55, 50).ToVector3(),
            new Color(26, 43, 140).ToVector3(),
            new Color(22, 22, 63).ToVector3(),
            new Color(169, 99, 77).ToVector3(),
            new Color(250, 221, 143).ToVector3(),
            new Color(255, 255, 255).ToVector3(),
            new Color(219, 160, 94).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Crimson => new Vector3[] //crimson slime; moonbee
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(62, 7, 6).ToVector3(),
            new Color(97, 168, 207).ToVector3(),
            new Color(24, 2, 2).ToVector3(),
            new Color(93, 12, 9).ToVector3(),
            new Color(235, 147, 129).ToVector3(),
            new Color(242, 214, 201).ToVector3(),
            new Color(217, 104, 80).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Corruption => new Vector3[] //corrupt slime; moonbee
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(29, 26, 53).ToVector3(),
            new Color(16, 14, 33).ToVector3(),
            new Color(39, 36, 66).ToVector3(),
            new Color(45, 40, 93).ToVector3(),
            new Color(97, 89, 164).ToVector3(),
            new Color(168, 163, 209).ToVector3(),
            new Color(61, 54, 138).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Hallow => new Vector3[] //hallow slime; moonbee
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(124, 45, 134).ToVector3(),
            new Color(85, 41, 91).ToVector3(),
            new Color(226, 89, 227).ToVector3(),
            new Color(241, 142, 222).ToVector3(),
            new Color(100, 212, 176).ToVector3(),
            new Color(255, 255, 255).ToVector3(),
            new Color(241, 242, 116).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] TheDragon => new Vector3[] //shen doragon; split
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(46, 44, 68).ToVector3(),
            new Color(106, 93, 117).ToVector3(),
            new Color(115, 149, 171).ToVector3(),
            new Color(217, 173, 74).ToVector3(),
            new Color(176, 74, 39).ToVector3(),
            new Color(146, 37, 30).ToVector3(),
            new Color(102, 20, 48).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Speevil => new Vector3[] //sonic.exe; split
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(46, 44, 68).ToVector3(),
            new Color(106, 93, 117).ToVector3(),
            new Color(115, 149, 171).ToVector3(),
            new Color(217, 173, 74).ToVector3(),
            new Color(176, 74, 39).ToVector3(),
            new Color(146, 37, 30).ToVector3(),
            new Color(102, 20, 48).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] HyperrealisticBloody => new Vector3[] //hyperrealistic bloody colores; split
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(12, 22, 28).ToVector3(),
            new Color(39, 52, 60).ToVector3(),
            new Color(80, 66, 48).ToVector3(),
            new Color(189, 198, 171).ToVector3(),
            new Color(254, 254, 254).ToVector3(),
            new Color(255, 98, 98).ToVector3(),
            new Color(193, 0, 0).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] AuricTesla => new Vector3[] //auric tesla armor n shit; split
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(31, 39, 65).ToVector3(),
            new Color(62, 62, 96).ToVector3(),
            new Color(98, 45, 59).ToVector3(),
            new Color(170, 96, 60).ToVector3(),
            new Color(123, 205, 237).ToVector3(),
            new Color(219, 148, 67).ToVector3(),
            new Color(169, 99, 61).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Pikmin => new Vector3[] //all the pikmin colors before the fourth game; split
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(62, 50, 83).ToVector3(),
            new Color(255, 0, 123).ToVector3(),
            new Color(149, 0, 255).ToVector3(),
            new Color(255, 255, 255).ToVector3(),
            new Color(255, 8, 0).ToVector3(),
            new Color(30, 0, 255).ToVector3(),
            new Color(255, 234, 0).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Flame => new Vector3[] //fire; triangle
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(246, 33, 0).ToVector3(),
            new Color(237, 111, 0).ToVector3(),
            new Color(249, 170, 0).ToVector3(),
            new Color(249, 243, 65).ToVector3(),
            new Color(255, 253, 252).ToVector3(),
            new Color(255, 224, 123).ToVector3(),
            new Color(255, 148, 25).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Poland => new Vector3[] //poland color palette; split
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(40, 42, 31).ToVector3(),
            new Color(62, 62, 62).ToVector3(),
            new Color(94, 98, 99).ToVector3(),
            new Color(145, 151, 163).ToVector3(),
            new Color(116, 116, 118).ToVector3(),
            new Color(62, 62, 62).ToVector3(),
            new Color(78, 78, 78).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Kindergarten => new Vector3[] //garten of banban sweep; split
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(0, 255, 11).ToVector3(),
            new Color(230, 255, 237).ToVector3(),
            new Color(227, 4, 17).ToVector3(),
            new Color(242, 158, 178).ToVector3(),
            new Color(111, 130, 222).ToVector3(),
            new Color(255, 117, 29).ToVector3(),
            new Color(21, 0, 255).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Daniel => new Vector3[] //this damn daniel gradient shows up at your front door; split
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(93, 80, 131).ToVector3(),
            new Color(117, 108, 161).ToVector3(),
            new Color(95, 111, 163).ToVector3(),
            new Color(91, 158, 201).ToVector3(),
            new Color(100, 176, 215).ToVector3(),
            new Color(179, 216, 232).ToVector3(),
            new Color(242, 242, 240).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Autumnal => new Vector3[] //dont listen to the name, its actually the amira palette; split
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(93, 47, 109).ToVector3(),
            new Color(172, 67, 114).ToVector3(),
            new Color(240, 94, 125).ToVector3(),
            new Color(253, 232, 214).ToVector3(),
            new Color(240, 181, 164).ToVector3(),
            new Color(255, 126, 97).ToVector3(),
            new Color(243, 67, 53).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Subworld => new Vector3[] //bereft vassal color palette; split
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(107, 64, 72).ToVector3(),
            new Color(150, 96, 87).ToVector3(),
            new Color(191, 143, 103).ToVector3(),
            new Color(225, 193, 131).ToVector3(),
            new Color(19, 225, 203).ToVector3(),
            new Color(87, 140, 142).ToVector3(),
            new Color(72, 100, 116).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };

        public static Vector3[] Test => new Vector3[]
        {
            new Color(0, 0, 0).ToVector3(),
            new Color(200, 20, 10).ToVector3(),
            new Color(235, 80, 20).ToVector3(),
            new Color(255, 100, 0).ToVector3(),
            new Color(255, 150, 0).ToVector3(),
            new Color(255, 100, 0).ToVector3(),
            new Color(235, 80, 0).ToVector3(),
            new Color(220, 20, 10).ToVector3(),
            new Color(0, 0, 0).ToVector3()
        };
    }
}
