using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Integration.Common
{
    public class Utility
    {
        /// <summary>
        /// Takes customer name field and splits it into first, middle and lastnames.
        /// </summary>
        /// <param name="customerName"></param>
        /// <param name="firstName"></param>
        /// <param name="middleName"></param>
        /// <param name="lastName"></param>
        public static void DecodeName(string customerName, out string firstName, out string middleName, out string lastName)
        {
            firstName = String.Empty;
            middleName = String.Empty;
            lastName = String.Empty;

            string[] splitName = customerName.Trim().Split();

            lastName = splitName[splitName.Length - 1].Trim();

            for (int i = 0; i < splitName.Length - 1; i++)
            {
                if (!String.IsNullOrWhiteSpace(splitName[i]))
                {
                    if (String.IsNullOrWhiteSpace(firstName) && i == 0)
                        firstName = splitName[i];
                    else
                        middleName += splitName[i] + " ";
                }
            }

            firstName = firstName.Trim();
            middleName = middleName.Trim();
            lastName = lastName.Trim();
        }

        public static string BuildNameOnCard(string firstNameIn, string middleNameIn, string lastNameIn)
        {
            string firstName = String.IsNullOrWhiteSpace(firstNameIn) ? String.Empty : firstNameIn.Trim() + " "
                        , middleNames = String.IsNullOrWhiteSpace(middleNameIn) ? String.Empty : middleNameIn.Trim() + " "
                        , lastName = String.IsNullOrWhiteSpace(lastNameIn) ? String.Empty : lastNameIn.Trim();

            int checks = 0;

            //Numba One: If all the names are greater than 25 characters
            while (firstName.Length +
                middleNames.Length +
                lastName.Length > 25)
            {
                switch (checks)
                {
                    case 0:
                        string tempInitials = String.Empty;
                        //Numba Two: Initialise the Middle name/s
                        foreach (var initial in middleNames.Trim().Split(' '))
                        {
                            tempInitials += initial.Substring(0, 1) + " ";
                        }
                        middleNames = tempInitials;
                        break;
                    case 1:
                        //Numba Three: Initialise First name
                        firstName = firstName.Substring(0, 1) + " ";
                        break;
                    case 2:
                        //Numba Four: Remove middle names/initials
                        middleNames = String.Empty;
                        break;
                    case 3:
                        //Numba Five: Remove first name/initial
                        firstName = String.Empty;
                        break;
                    case 4:
                        //Numba Six: First 25 characters of last name
                        lastName = lastName.Substring(0, 25);
                        break;
                }
                checks++;
            }

            return String.Format("{0}{1}{2}", firstName, middleNames, lastName);
        }
    }
}
