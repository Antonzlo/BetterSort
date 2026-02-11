using BetterSort.Accuracy.External;
using BetterSort.Accuracy.Sorter;
using BetterSort.Common.Models;
using BetterSort.Common.Test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace BetterSort.Accuracy.Test {

  [TestClass]
  public class StarRatingSorterTest {

    [TestMethod]
    public void TestNull() {
      var result = StarRatingSorter.SortInternal(null, () => null, new List<LevelRecord>());
      Assert.IsNull(result.Result);
    }

    [TestMethod]
    public void TestEmpty() {
      var result = StarRatingSorter.SortInternal(new List<ILevelPreview>(), () => null, new List<LevelRecord>());
      CollectionAssert.AreEqual(new List<ILevelPreview>(), result.Result!.Levels.ToList());
    }

    [TestMethod]
    public void TestSingle() {
      var levels = new List<ILevelPreview>() { new MockPreview("custom_level_0000000000000000000000000000000000000000") };
      var records = new SorterData() {
        {
          "custom_level_0000000000000000000000000000000000000000",
          new() {
            { ("Standard", RecordDifficulty.ExpertPlus), new ScoreRecord(0.90292, 5.5)  },
          }
        },
      };

      var result = StarRatingSorter.SortInternal(levels, () => records, new List<LevelRecord>()).Result;

      CollectionAssert.AreEqual(
        new List<string> { "custom_level_0000000000000000000000000000000000000000" },
        result?.Levels.Select(x => x.LevelId).ToList()
      );
      CollectionAssert.AreEqual(new List<(string Label, int Index)>() { ("5.50★", 0) }, result?.Legend.ToList());
    }

    [TestMethod]
    public void TestStarRatingSorting() {
      var levels = new List<ILevelPreview>() {
        new MockPreview("custom_level_1111111111111111111111111111111111111111"),
        new MockPreview("custom_level_0000000000000000000000000000000000000000"),
      };
      var records = new SorterData() {
        {
          "custom_level_0000000000000000000000000000000000000000",
          new() {
            { ("Standard", RecordDifficulty.ExpertPlus) , new ScoreRecord(0.90292, 5.5) },
          }
        },
        {
          "custom_level_1111111111111111111111111111111111111111",
          new() {
            { ("Standard", RecordDifficulty.ExpertPlus), new ScoreRecord(0.80004, 8.2) },
            { ("Standard", RecordDifficulty.Expert), new ScoreRecord(0.92004, 6.8) },
          }
        },
      };

      var result = StarRatingSorter.SortInternal(levels, () => records, new List<LevelRecord>()).Result;

      // Should be sorted by stars in descending order: 8.2, 6.8, 5.5
      CollectionAssert.AreEqual(
        new List<string> {
          "custom_level_1111111111111111111111111111111111111111", // 8.2 stars
          "custom_level_1111111111111111111111111111111111111111", // 6.8 stars
          "custom_level_0000000000000000000000000000000000000000", // 5.5 stars
        },
        result!.Levels.Select(x => x.LevelId).ToList()
      );
      CollectionAssert.AreEqual(new List<(string Label, int Index)>() { ("8.20★", 0), ("6.80★", 1), ("5.50★", 2) }, result.Legend.ToList());
    }

    [TestMethod]
    public void TestDifficultySplitting() {
      // Test that each difficulty creates a separate entry
      var levels = new List<ILevelPreview>() {
        new MockPreview("custom_level_AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"),
      };
      var records = new SorterData() {
        {
          "custom_level_AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
          new() {
            { ("Standard", RecordDifficulty.Easy), new ScoreRecord(0.95, 1.0) },
            { ("Standard", RecordDifficulty.Normal), new ScoreRecord(0.93, 2.0) },
            { ("Standard", RecordDifficulty.Hard), new ScoreRecord(0.91, 3.0) },
            { ("Standard", RecordDifficulty.Expert), new ScoreRecord(0.89, 4.0) },
            { ("Standard", RecordDifficulty.ExpertPlus), new ScoreRecord(0.87, 5.0) },
          }
        },
      };

      var result = StarRatingSorter.SortInternal(levels, () => records, new List<LevelRecord>()).Result;

      // All 5 difficulties should be present, sorted by stars (descending)
      Assert.AreEqual(5, result!.Levels.Count());
      
      // All should be the same song
      CollectionAssert.AreEqual(
        new List<string> {
          "custom_level_AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
          "custom_level_AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
          "custom_level_AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
          "custom_level_AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
          "custom_level_AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
        },
        result.Levels.Select(x => x.LevelId).ToList()
      );
    }

    [TestMethod]
    public void TestMixedMaps() {
      // Test with maps with and without star ratings
      var levels = new List<ILevelPreview>() {
        new MockPreview("custom_level_WITH_RATING_1111111111111111111111111"),
        new MockPreview("custom_level_WITHOUT_RATING_222222222222222222222"),
        new MockPreview("custom_level_WITH_RATING_3333333333333333333333333"),
      };
      var records = new SorterData() {
        {
          "custom_level_WITH_RATING_1111111111111111111111111",
          new() {
            { ("Standard", RecordDifficulty.ExpertPlus), new ScoreRecord(0.90, 7.5) },
          }
        },
        {
          "custom_level_WITH_RATING_3333333333333333333333333",
          new() {
            { ("Standard", RecordDifficulty.Expert), new ScoreRecord(0.85, 9.2) },
          }
        },
        // custom_level_WITHOUT_RATING_222222222222222222222 has no record
      };

      var result = StarRatingSorter.SortInternal(levels, () => records, new List<LevelRecord>()).Result;

      // Maps with ratings should come first (sorted by stars), then maps without
      CollectionAssert.AreEqual(
        new List<string> {
          "custom_level_WITH_RATING_3333333333333333333333333", // 9.2 stars
          "custom_level_WITH_RATING_1111111111111111111111111", // 7.5 stars
          "custom_level_WITHOUT_RATING_222222222222222222222",  // no rating
        },
        result!.Levels.Select(x => x.LevelId).ToList()
      );
    }
  }
}
