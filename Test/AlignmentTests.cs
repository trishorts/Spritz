﻿using NUnit.Framework;
using System.IO;
using ToolWrapperLayer;
using WorkflowLayer;
using System.Collections.Generic;

namespace Test
{
    [TestFixture]
    public class AlignmentTests
    {
        [Test]
        public void CountReads()
        {
            Assert.AreEqual(3970, new FastqProperties(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "mapper.fastq")).ReadCount);
        }

        [Test, Order(1)]
        public void SubsetReadsCheck()
        {
            string[] new_files = new string[0];

            STARWrapper.SubsetFastqs(
                TestContext.CurrentContext.TestDirectory,
                new string[]
                {
                    Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "read1.fastq"),
                    Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "read2.fastq")
                },
                100,
                TestContext.CurrentContext.TestDirectory,
                out new_files);

            foreach (string file in new_files)
            {
                Assert.IsTrue(File.Exists(file));
                Assert.IsTrue(new FileInfo(file).Length > 0);
            }
        }

        [Test, Order(2)]
        public void TestGenomeGenerate()
        {
            WrapperUtility.GenerateAndRunScript(Path.Combine(TestContext.CurrentContext.TestDirectory, "scripts", "genomeGenerate.bash"),
                STARWrapper.GenerateGenomeIndex(TestContext.CurrentContext.TestDirectory,
                1,
                Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "sampleGenomeDir"),
                new string[] { Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "chr1_sample.fa") },
                Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "chr1_one_transcript.gtf"))).WaitForExit();
            Assert.IsTrue(Directory.Exists(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "sampleGenomeDir")));
            Assert.IsTrue(File.Exists(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "sampleGenomeDir", "SA")));
        }

        [Test, Order(3)]
        public void TestAlign()
        {
            WrapperUtility.GenerateAndRunScript(Path.Combine(TestContext.CurrentContext.TestDirectory, "scripts", "alignReads.bash"),
            STARWrapper.BasicAlignReadCommands
            (
                TestContext.CurrentContext.TestDirectory,
                1,
                Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "sampleGenomeDir"),
                new string[]
                {
                    Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData","read1.fastq"),
                    Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData","read2.fastq")
                },
                Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "r.")
            )).WaitForExit();
            Assert.IsTrue(File.Exists(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "r.Aligned.out.bam")));
        }

        [Test]
        public void DebugStarThreadCount()
        {
            STARAlignmentFlow s = new STARAlignmentFlow();
            s.OutputPrefixes = new List<string> { Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "read1-trimmed-pair1") };
            Assert.AreEqual(2, s.GetDebuggedThreadCount());
        }

        [Test, Order(1)]
        public void TophatAlign()
        {
            TopHatWrapper.GenerateBowtieIndex(
                TestContext.CurrentContext.TestDirectory,
                Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "chr1_sample.fa"),
                out string bowtieIndexPrefix);
            Assert.IsTrue(TopHatWrapper.BowtieIndexExists(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "chr1_sample.fa")));

            TopHatWrapper.Align(
                TestContext.CurrentContext.TestDirectory,
                bowtieIndexPrefix,
                8,
                new string[]
                {
                    Path.Combine(TestContext.CurrentContext.TestDirectory,"TestData", "mapper.fastq"),
                },
                true,
                out string tophatOutDirectory
                );
            Assert.IsTrue(File.Exists(Path.Combine(tophatOutDirectory, TopHatWrapper.TophatAcceptedHitsFilename)));
            Assert.IsTrue(File.Exists(Path.Combine(tophatOutDirectory, TopHatWrapper.TophatAlignmentSummaryFilename)));
            Assert.IsTrue(File.Exists(Path.Combine(tophatOutDirectory, TopHatWrapper.TophatDeletionsBEDFilename)));
            Assert.IsTrue(File.Exists(Path.Combine(tophatOutDirectory, TopHatWrapper.TophatInsertionsBEDFilename)));
            Assert.IsTrue(File.Exists(Path.Combine(tophatOutDirectory, TopHatWrapper.TophatJunctionsBEDFilename)));
        }
    }
}