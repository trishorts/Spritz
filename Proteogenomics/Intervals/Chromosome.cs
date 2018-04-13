﻿using Bio;
using System;

namespace Proteogenomics
{
    public class Chromosome
        : Interval
    {
        public ISequence Sequence { get; set; }

        public string FriendlyName { get; set; }

        public bool Mitochondrial { get; set; }

        public Chromosome(ISequence sequence, Genome genome)
            : base(genome, sequence.ID, "+", 1, sequence.Count, null)
        {
            Sequence = sequence;
            FriendlyName = GetFriendlyChromosomeName(ChromosomeID);
            Mitochondrial = FriendlyName.StartsWith("M") || FriendlyName.StartsWith("chrM");
        }

        public static string GetFriendlyChromosomeName(string chromosomeID)
        {
            return chromosomeID.Split(new string[] { ProteogenomicsUtility.ENSEMBL_FASTA_HEADER_DELIMETER }, StringSplitOptions.None)[0];
        }
    }
}