using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Device.Location;
using RhitMobile.ObjectModel;
using Microsoft.Phone.Controls.Maps;

namespace RhitMobile {
	public class Locations {

        public static RhitLocation LOCATION_RHIT = new RhitLocation(39.4820263, -87.3248677) {
            Label = ""
        };

        public static RhitLocation LOCATION_HATFIELD = new RhitLocation(39.48222308900057, -87.32272186144638) {
            Label = "Hatfeild",
			Locations = new LocationCollection() {
				new GeoCoordinate(39.48222308900057,-87.32272186144638),
				new GeoCoordinate(39.482258282181824,-87.32257433995056),
				new GeoCoordinate(39.48247358125593,-87.32227929695892),
				new GeoCoordinate(39.48246116017362,-87.32226052149582),
				new GeoCoordinate(39.48252533574169,-87.32218273743439),
				new GeoCoordinate(39.48237214235214,-87.32190646990585),
				new GeoCoordinate(39.482330738675415,-87.32196279629517),
				new GeoCoordinate(39.48224586106107,-87.3218233214264),
				new GeoCoordinate(39.48205333340555,-87.32201107605744),
				new GeoCoordinate(39.48203056128202,-87.32198961838532),
				new GeoCoordinate(39.48190634956786,-87.32217200859833),
				new GeoCoordinate(39.481848384025355,-87.32218273743439),
				new GeoCoordinate(39.481989157401976,-87.32261993750382),
			}
		};

        public static RhitLocation LOCATION_HADLEY = new RhitLocation(39.4829455807401, -87.32426949604798) {
            Label = "Hadley",
			Locations = new LocationCollection() {
				new GeoCoordinate(39.4829455807401,-87.32426949604798),
				new GeoCoordinate(39.483015966353605,-87.3239503131752),
				new GeoCoordinate(39.482846212693865,-87.32388594015885),
				new GeoCoordinate(39.48277168656604,-87.32419975861359),
			}
		};

        public static RhitLocation LOCATION_OLIN = new RhitLocation(39.48288968623158, -87.32536651953507) {
            Label = "Olin",
			Locations = new LocationCollection() {
                new GeoCoordinate(39.48288968623158,-87.32536651953507),
                new GeoCoordinate(39.482978703390614,-87.32498564585495),
                new GeoCoordinate(39.48293522990857,-87.32496418818283),
                new GeoCoordinate(39.48307186076065,-87.32435264452744),
                new GeoCoordinate(39.48263505512423,-87.32418902977753),
                new GeoCoordinate(39.48251498484772,-87.32473620041657),
                new GeoCoordinate(39.48284000218626,-87.32484885319519),
                new GeoCoordinate(39.48278410759288,-87.32514657839585),
                new GeoCoordinate(39.48262677442215,-87.3250848875885),
                new GeoCoordinate(39.48256880948006,-87.32526727780152),
			}
		};

        public static RhitLocation LOCATION_MOENCH = new RhitLocation(39.48386886871469, -87.32421853407669) {
            Label = "Moench",
			Locations = new LocationCollection() {
                new GeoCoordinate(39.48386886871469,-87.32421853407669),
                new GeoCoordinate(39.48398272624822,-87.32370354994583),
                new GeoCoordinate(39.48299319454528,-87.32332535847473),
                new GeoCoordinate(39.48287312488707,-87.32386180027771),
			}
		};

        public static RhitLocation LOCATION_CRAPO = new RhitLocation(39.4837632915625, -87.32470133169937) {
            Label = "Crapo",
			Locations = new LocationCollection() {
                new GeoCoordinate(39.4837632915625,-87.32470133169937),
                new GeoCoordinate(39.48386058815953,-87.3242909537201),
                new GeoCoordinate(39.48369911713676,-87.32422389849472),
                new GeoCoordinate(39.483603890460415,-87.32464500531006),
			}
		};

        public static RhitLocation LOCATION_LOGAN = new RhitLocation(39.48345276959767, -87.3250848875885) {
            Label = "Logan",
			Locations = new LocationCollection() {
                new GeoCoordinate(39.48345276959767,-87.3250848875885),
                new GeoCoordinate(39.48353764573909,-87.32467450960922),
                new GeoCoordinate(39.48339273519121,-87.32461013659287),
                new GeoCoordinate(39.48330164840657,-87.32502319678116),
			}
		};

        public static RhitLocation LOCATION_ROTZ_LAB = new RhitLocation(39.483735563451596, -87.32341279496768) {
            Label = "Rotz",
			Locations = new LocationCollection() {
                new GeoCoordinate(39.483735563451596,-87.32341279496768),
                new GeoCoordinate(39.48379870277332,-87.32315530290225),
                new GeoCoordinate(39.48360514434323,-87.32307617773631),
                new GeoCoordinate(39.48354303991996,-87.32333635201076),
			}
		};

        public static RhitLocation LOCATION_MYERS = new RhitLocation(39.4840026113389, -87.32352812995532) {
            Label = "Meyers",
			Locations = new LocationCollection() {
                new GeoCoordinate(39.4840026113389,-87.32352812995532),
                new GeoCoordinate(39.484074030948925,-87.32319285382846),
                new GeoCoordinate(39.48417132711121,-87.32322772254565),
                new GeoCoordinate(39.484227220589695,-87.32297559489825),
                new GeoCoordinate(39.483625845805335,-87.32271139731029),
                new GeoCoordinate(39.4836020391234,-87.32280661573031),
                new GeoCoordinate(39.48362377565939,-87.32282002677539),
                new GeoCoordinate(39.48359582868327,-87.32293402065852),
                new GeoCoordinate(39.4839156656277,-87.32306276669124),
                new GeoCoordinate(39.483827684737854,-87.323469121357),
			}
		};

        public static RhitLocation LOCATION_FACILITIES = new RhitLocation(39.48506332846795, -87.3222216294651) {
            Label = "Facilities",
			Locations = new LocationCollection() {
                new GeoCoordinate(39.48506332846795,-87.3222216294651),
                new GeoCoordinate(39.48505711815832,-87.32151352628517),
                new GeoCoordinate(39.484845967301496,-87.32152157291222),
                new GeoCoordinate(39.48485424773933,-87.32223235830116),
			}
		};

        public static RhitLocation LOCATION_RECYCLING_CENTER = new RhitLocation(39.48467540185932, -87.32034166564563) {
            Label = "Recycling Center",
			Locations = new LocationCollection() {
                new GeoCoordinate(39.48467540185932,-87.32034166564563),
                new GeoCoordinate(39.48468368231748,-87.31982936372378),
                new GeoCoordinate(39.484565685695806,-87.31982936372378),
                new GeoCoordinate(39.48456258051885,-87.32034032454112),
			}
		};

        public static RhitLocation LOCATION_ATO = new RhitLocation(39.4843211925432, -87.3214451299553) {
            Label = "ATO",
			Locations = new LocationCollection() {
                new GeoCoordinate(39.4843211925432,-87.3214451299553),
                new GeoCoordinate(39.4843915767645,-87.32135125263977),
                new GeoCoordinate(39.484207335563816,-87.3211044894104),
                new GeoCoordinate(39.48420526543517,-87.32098915442276),
                new GeoCoordinate(39.484122460239696,-87.32098647221375),
                new GeoCoordinate(39.484124530370785,-87.32105620964813),
                new GeoCoordinate(39.483944428735576,-87.32105352743912),
                new GeoCoordinate(39.483944428735576,-87.32122518881607),
                new GeoCoordinate(39.48414730180871,-87.32121714218903),
			}
		};

        public static RhitLocation LOCATION_TRIANGLE = new RhitLocation(39.48376639677569, -87.32135929926682) {
            Label = "Triangle",
			Locations = new LocationCollection() {
                new GeoCoordinate(39.48376639677569,-87.32135929926682),
                new GeoCoordinate(39.48376846691738,-87.32091941698837),
                new GeoCoordinate(39.48363804787073,-87.32091941698837),
                new GeoCoordinate(39.48363804787073,-87.32103743418503),
                new GeoCoordinate(39.48339584042094,-87.32104011639404),
                new GeoCoordinate(39.48339584042094,-87.32121445998001),
                new GeoCoordinate(39.48363804787073,-87.32121714218903),
                new GeoCoordinate(39.48363804787073,-87.32135125263977),
			}
		};

        public static RhitLocation LOCATION_LAMBDA_CHI = new RhitLocation(39.483205386107315, -87.32116081579971) {
            Label = "Lambda Chi",
			Locations = new LocationCollection() {
                new GeoCoordinate(39.483205386107315,-87.32116081579971),
                new GeoCoordinate(39.483205386107315,-87.32086845501709),
                new GeoCoordinate(39.48293005449279,-87.32086577280808),
                new GeoCoordinate(39.48293212465938,-87.321211777771),
                new GeoCoordinate(39.483035632909925,-87.32121714218903),
                new GeoCoordinate(39.48303770307335,-87.32116349800873),
			}
		};

        public static RhitLocation LOCATION_SKINNER = new RhitLocation(39.48260917792758, -87.32079603537369) {
            Label = "Skinner",
			Locations = new LocationCollection() {
                new GeoCoordinate(39.48260917792758,-87.32079603537369),
                new GeoCoordinate(39.48260710775138,-87.32066997154999),
                new GeoCoordinate(39.48216408862803,-87.32067801817703),
                new GeoCoordinate(39.4821620184386,-87.3207987175827),
			}
		};

        public static RhitLocation LOCATION_CIRLCE_K = new RhitLocation(39.481998473277834, -87.32107230290222) {
            Label = "Circle K",
			Locations = new LocationCollection() {
                new GeoCoordinate(39.481998473277834,-87.32107230290222),
                new GeoCoordinate(39.48204194734543,-87.32086577280808),
                new GeoCoordinate(39.48193222702732,-87.32082285746384),
                new GeoCoordinate(39.48188254229803,-87.32102938755799),
			}
		};

        public static RhitLocation LOCATION_PUBLIC_SAFETY = new RhitLocation(39.48194257800814, -87.32053317889023) {
            Label = "Public Safety",
			Locations = new LocationCollection() {
                new GeoCoordinate(39.48194257800814,-87.32053317889023),
                new GeoCoordinate(39.481996403083464,-87.32032396658707),
                new GeoCoordinate(39.481913595258,-87.32028641566086),
                new GeoCoordinate(39.481859770118575,-87.32049562796402),
			}
		};

        public static RhitLocation LOCATION_BSB = new RhitLocation(39.48279135319182, -87.32565351589966) {
            Label = "BSB",
			Locations = new LocationCollection() {
                new GeoCoordinate(39.48279135319182,-87.32565351589966),
                new GeoCoordinate(39.482760300623774,-87.3255515919571),
                new GeoCoordinate(39.48260296739921,-87.32562132939148),
                new GeoCoordinate(39.48259261651669,-87.32557841404724),
                new GeoCoordinate(39.4821723693856,-87.32579030855942),
                new GeoCoordinate(39.48220963278061,-87.32600756748963),
                new GeoCoordinate(39.482327633399755,-87.32594587668228),
			}
		};

	}
}
