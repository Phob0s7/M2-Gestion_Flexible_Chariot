﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace M2_Gestion_Flexible_Chariot
{
    /// <summary>
    /// Classe qui contient la gestion des lots.
    /// </summary>
    public class GestionLots
    {
        /// <summary>
        /// Permet d'afficher le menu des lots.
        /// </summary>
        public static void AfficherMenuLots()
        {
            Console.Clear();
            Console.WriteLine("__________________________________________________");
            Console.WriteLine("\t        *** Menu lots ***                       ");
            Console.WriteLine("__________________________________________________");
            Console.WriteLine("\n1. Création de lots");
            Console.WriteLine("2. Affichage de lots");
            Console.WriteLine("\n3. Revenir au menu principal");
            Console.WriteLine("__________________________________________________");
        }

        /// <summary>
        /// Permet de saisir le choix auprès de l'utilisateur en fonction du "Menu lots.
        /// </summary>
        public static void ChoisirMenuLots()
        {
            string choixMenuLots = "";
            bool saisieInvalide = false;

            do
            {
                saisieInvalide = false;

                Console.Write("Votre choix : ");
                choixMenuLots = Console.ReadLine();

                switch (choixMenuLots)
                {
                    case "1":
                        CréationLots();
                        AfficherMenuLots();
                        ChoisirMenuLots();
                        break;
                    case "2":
                        AfficherLots();
                        GestionMenuPrincipale.AttenteSaisieUtilisateur();
                        AfficherMenuLots();
                        ChoisirMenuLots();
                        break;
                    case "3":
                        GestionMenuPrincipale.AfficherMenuPrincipale();
                        break;
                    default:
                        saisieInvalide = true;
                        GestionMenuPrincipale.AfficherErreurSaisieMenu();
                        break;
                }
            } while (saisieInvalide == true);
        }

        /// <summary>
        /// Permet de créer un lot.
        /// </summary>
        public static void CréationLots()
        {
            const int ID_STATUT_EN_ATTENTE = 1;

            int nbreLots = 1;
            int nbreAjout = 0;
            int qtePièceRéalisée = 0;
            int qtePièceAProduire = 0;
            string IDRecette = "";
            string choixCréationLots = "";

            List<string> listeIDRecette = new List<string>();

            DateTime dateTime = DateTime.Now;

            do
            {
                Console.Clear();
                Console.WriteLine("               Lot n° " + nbreLots);
                qtePièceAProduire = SaisirQtePiècesAProduire();
                GestionRecettes.AfficherRecettes();

                do
                {
                    Console.Write("\nID de la recette à associer à ce lot : ");
                    IDRecette = Console.ReadLine(); ;

                    try
                    {
                        using (MySqlCommand cmd = GestionBaseDeDonnée.GetMySqlConnection().CreateCommand())
                        {
                            listeIDRecette = GestionRecettes.StockerIDRecette();

                            if (listeIDRecette.Contains(IDRecette))
                            {
                                cmd.CommandText = "INSERT INTO lot (LOT_QtePieceRealisee, LOT_QtePieceAProduire, LOT_DateCreation, REC_ID, STA_ID) VALUES (@pièceRéalisée, @pièceProduire, @date, @RECID, @STAID);";

                                cmd.Parameters.AddWithValue("@pièceRéalisée", qtePièceRéalisée);
                                cmd.Parameters.AddWithValue("@pièceProduire", qtePièceAProduire);
                                cmd.Parameters.AddWithValue("@date", dateTime);
                                cmd.Parameters.AddWithValue("@RECID", IDRecette);
                                cmd.Parameters.AddWithValue("@STAID", ID_STATUT_EN_ATTENTE);

                                choixCréationLots = GestionPas.ErreurSaisirChoix("\nVoulez-vous créer un nouveau lot ? (O/N) ");
                                nbreAjout += cmd.ExecuteNonQuery();
                                nbreLots++;
                            }

                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\nL'ID de la recette n'existe pas, veuillez réessayer.");
                                Console.ResetColor();
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        Console.Write("\nAttention il y a eu le problème suivant : ");
                        Console.Write(ex.Message);
                        Console.Write("\nVeuillez appuyer sur une touche pour continuer...");
                        Console.ReadKey();
                        Console.Write("\n\n");
                    }
                } while (!listeIDRecette.Contains(IDRecette));
            } while (choixCréationLots != "N");

            Console.WriteLine("\nNombre de lots créés : {0}", nbreAjout);
            GestionMenuPrincipale.AttenteSaisieUtilisateur();
        }

        /// <summary>
        /// Saisie la quantité de pièce à produire.
        /// </summary>
        public static int SaisirQtePiècesAProduire()
        {
            string saisieUtilisateur = "";
            bool saisieValide = false;
            int qtePièceAProduire = 0;

            do
            {
                Console.Write("\nQuantité de pièces à produire : ");
                saisieUtilisateur = Console.ReadLine();

                if (!int.TryParse(saisieUtilisateur, out qtePièceAProduire))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nVeuillez saisir une valeur correcte (nombre).");
                    Console.ResetColor();
                }

                else
                {
                    qtePièceAProduire = int.Parse(saisieUtilisateur);
                    saisieValide = true;
                }

            } while (saisieValide == false);

            return qtePièceAProduire;
        }
       
        /// <summary>
        /// Permet d'afficher la liste des lots.
        /// </summary>
        public static void AfficherLots()
        {
            try
            {
                using (MySqlCommand cmd = GestionBaseDeDonnée.GetMySqlConnection().CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM lot";
                    string colonnes = "\nID du lot {0,-4} Quantité réalisée {0,-4} Quantité à produire {0,-4} Date de création {0,-4} ID de la recette {0,-4} Statut du lot\n";
                    Console.Write(string.Format(colonnes, "", "", ""));
                    Console.WriteLine("");

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        int compteur = 0;
                        while (reader.Read())
                        {
                            Console.Write(string.Format("{0,-15}", reader["LOT_ID"]));
                            Console.Write(string.Format("{0,-23}", reader["LOT_QtePieceRealisee"]));
                            Console.Write(string.Format("{0,-25}", reader["LOT_QtePieceAProduire"]));
                            Console.Write(string.Format("{0,-22}", reader["LOT_DateCreation"]));
                            Console.Write(string.Format("{0,-22}", reader["REC_ID"]));

                            if ((int)reader["STA_ID"] == 1)
                            {
                                Console.Write(string.Format("{0,0}", "En attente de production"));
                            }

                            else if ((int)reader["STA_ID"] == 2)
                            {
                                Console.Write(string.Format("{0,0}", "En cours de production"));
                            }

                            else
                            {
                                Console.Write(string.Format("{0,0}", "Terminée"));
                            }
                            Console.WriteLine("");
                            compteur++;
                        }

                        Console.Write("\n{0} lot(s) affiché(s).\n", compteur);
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.Write("\nAttention il y a eu le problème suivant : ");
                Console.Write(ex.Message);
                Console.Write("\nVeuillez appuyer sur une touche pour continuer...");
                Console.ReadKey();
                Console.Write("\n\n");
            }
        }
    }
}
