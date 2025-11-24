using System.Collections.Generic;

namespace Starter;

public record GameDescription(string Mission, string Explanation) {
	public static Dictionary<GameControl.GameTypeE, GameDescription> GameDescriptions = new() {
		{
			GameControl.GameTypeE.Asteroids,
			new (
				Mission: """
				         Ziel:
				         * Zerstöre die Asteroiden
				           bevor sie dich erwischen
				         * Versuche so lange
				           wie möglich zu überleben
				         """,
				Explanation: """
				             So funktioniert’s:
				             * Versuche nicht getroffen zu werden
				               (dein Schiff ist nicht unzerstörbar!)
				             * Sammel Punkte für das Zerstören von Asteroiden
				             """
			)
		},
		{
			GameControl.GameTypeE.BitKnight,
			new (
				Mission: """
				         Ziel:
				         * Erreiche das Ziel
				           so schnell du kannst.
				         * Zeit ist alles.
				         """,
				Explanation: """
				             So funktioniert’s:
				             * Tempo ist hier gefragt sei der schnellste
				               und gewinne einen unserer Preise
				             * Meide Gefahren: Gegner und Abgründe
				               setzen dich an den Anfang zurück!
				             """
			)
		},
		{
			GameControl.GameTypeE.Pong,
			new (
				Mission: """
				         Ziel:
				         * Tretet alleine gegen die KI an
				           oder spielt gegen eure Freunde
				           im 2-Player mode und messt euer Können
				         """,
				Explanation: """
				             So funktionierts:
				             * Bewege den jeweiligen Joystick hoch
				               oder runter um den Schläger zu bewegen
				             * Führe deinen Schläger vor den Ball
				               sodass dieser zurück fliegt
				             """
			)
		}
	};
}