using System;
using System.Security.Cryptography;

namespace Tripod.Ioc.Cryptography
{
    [UsedImplicitly]
    public class RngCryptoSecretCreator : ICreateSecrets
    {
        private const string DefaultLowercaseChars = "abcdefgijkmnopqrstwxyz";
        private const string DefaultUppercaseChars = "ABCDEFGHJKLMNPQRSTWXYZ";
        private const string DefaultNumericChars = "23456789";

        string ICreateSecrets.CreateSecret(int minLength, int maxLength)
        {
            return CreateSecret(minLength, maxLength);
        }

        string ICreateSecrets.CreateSecret(int exactLength)
        {
            return CreateSecret(exactLength, exactLength);
        }

        private static string CreateSecret(int minLength, int maxLength)
        {
            // code from http://stackoverflow.com/questions/4768712/create-a-12-character-secret
            // Make sure that input parameters are valid.
            if (minLength < 1)
                throw new ArgumentOutOfRangeException("minLength",
                    string.Format(Resources.Exception_ArgumentOutOfRange_CannotBeLessThan, 1));
            if (maxLength < minLength)
                throw new ArgumentOutOfRangeException("maxLength",
                    string.Format(Resources.Exception_ArgumentOutOfRange_CannotBeLessThan, "minLength"));

            // Create a local array containing supported secret characters
            // grouped by types. You can remove character groups from this
            // array, but doing so will weaken the secret strength.
            var charGroups = new[]
            {
                DefaultLowercaseChars.ToCharArray(),
                DefaultUppercaseChars.ToCharArray(),
                DefaultNumericChars.ToCharArray()
            };

            // Use this array to track the number of unused characters in each
            // character group.
            var charsLeftInGroup = new int[charGroups.Length];

            // Initially, all characters in each group are not used.
            for (var i = 0; i < charsLeftInGroup.Length; i++)
                charsLeftInGroup[i] = charGroups[i].Length;

            // Use this array to track (iterate through) unused character groups.
            var leftGroupsOrder = new int[charGroups.Length];

            // Initially, all character groups are not used.
            for (var i = 0; i < leftGroupsOrder.Length; i++)
                leftGroupsOrder[i] = i;

            // Because we cannot use the default randomizer, which is based on the
            // current time (it will produce the same "random" number within a
            // second), we will use a random number generator to seed the
            // randomizer.

            // Use a 4-byte array to fill it with random bytes and convert it then
            // to an integer value.
            var randomBytes = new byte[4];

            // Generate 4 random bytes.
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            // Convert 4 bytes into a 32-bit integer value.
            var seed = (randomBytes[0] & 0x7f) << 24 |
                        randomBytes[1] << 16 |
                        randomBytes[2] << 8 |
                        randomBytes[3];

            // Now, this is real randomization.
            var random = new Random(seed);

            // This array will hold secret characters.
            // Allocate appropriate memory for the secret.
            var secret = minLength < maxLength ? new char[random.Next(minLength, maxLength + 1)] : new char[minLength];

            // Index of the last non-processed group.
            var lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            // Generate secret characters one at a time.
            for (var i = 0; i < secret.Length; i++)
            {
                // Index which will be used to track not processed character groups.
                // If only one character group remained unprocessed, process it;
                // otherwise, pick a random character group from the unprocessed
                // group list. To allow a special character to appear in the
                // first position, increment the second parameter of the Next
                // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                var nextLeftGroupsOrderIdx = lastLeftGroupsOrderIdx == 0
                    ? 0 : random.Next(0, lastLeftGroupsOrderIdx);

                // Index of the next character group to be processed.
                // Get the actual index of the character group, from which we will
                // pick the next character.
                var nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                // Index of the last non-processed character in a group.
                // Get the index of the last unprocessed characters in this group.
                var lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                // Index of the next character to be added to secret.
                // If only one unprocessed character is left, pick it; otherwise,
                // get a random character from the unused character list.
                var nextCharIdx = lastCharIdx == 0 ? 0 : random.Next(0, lastCharIdx + 1);

                // Add this character to the secret.
                secret[i] = charGroups[nextGroupIdx][nextCharIdx];

                // If we processed the last character in this group, start over.
                if (lastCharIdx == 0)
                    charsLeftInGroup[nextGroupIdx] = charGroups[nextGroupIdx].Length;
                // There are more unprocessed characters left.
                else
                {
                    // Swap processed character with the last unprocessed character
                    // so that we don't pick it until we process all characters in
                    // this group.
                    if (lastCharIdx != nextCharIdx)
                    {
                        var temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] =
                                    charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }
                    // Decrement the number of unprocessed characters in
                    // this group.
                    charsLeftInGroup[nextGroupIdx]--;
                }

                // If we processed the last group, start all over.
                if (lastLeftGroupsOrderIdx == 0)
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                // There are more unprocessed groups left.
                else
                {
                    // Swap processed group with the last unprocessed group
                    // so that we don't pick it until we process all groups.
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        var temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] = leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }
                    // Decrement the number of unprocessed groups.
                    lastLeftGroupsOrderIdx--;
                }
            }

            return new string(secret);
        }
    }
}
