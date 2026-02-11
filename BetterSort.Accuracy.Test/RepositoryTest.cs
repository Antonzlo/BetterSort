global using SorterData = System.Collections.Generic.Dictionary<
  string,
  System.Collections.Generic.Dictionary<(string Type, BetterSort.Common.Models.RecordDifficulty), BetterSort.Accuracy.External.ScoreRecord>
>;
using BetterSort.Accuracy.External;
using BetterSort.Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BetterSort.Accuracy.Test {

  [TestClass]
  public class RepositoryTest {
    private static readonly SorterData _emptyRecords = [];

    private static readonly SorterData _singleRecords = new() {
      {
        "custom_level_000",
        new() {
          { new ("Standard", RecordDifficulty.ExpertPlus), new ScoreRecord(0.90292, 5.5) },
        }
      }
    };

    private static readonly SorterData _combinedRecords = new() {
      {
        "custom_level_111",
        new() {
          { new ("Standard", RecordDifficulty.ExpertPlus), new ScoreRecord(0.90292, 6.2) },
          { new ("Standard", RecordDifficulty.Expert), new ScoreRecord(0.92192, 5.1) },
        }
      },
    };

    private static readonly SorterData _doubleRecords = new() {
      {
        "custom_level_222",
        new() {
          { new ("Lawless", RecordDifficulty.Hard), new ScoreRecord(0.91000, 4.8) },
        }
      },
      {
        "custom_level_111",
        new() {
          { new ("Standard", RecordDifficulty.ExpertPlus), new ScoreRecord(0.90292, 6.2) },
          { new ("Standard", RecordDifficulty.Expert), new ScoreRecord(0.92192, 5.1) },
        }
      },
    };

    private static readonly DateTime _fixedTime = DateTime.Parse("2022-03-01T00:00:00Z");

    private static readonly string _version = typeof(AccuracyRepository).Assembly.GetName().Version.ToString();

    [TestMethod]
    public void TestSaveEmpty() {
      var (json, data) = AccuracyRepository.GetPersistentData(_emptyRecords, _fixedTime);

      Assert.AreEqual($$"""
{
  "bestRecords": [],
  "lastRecordAt": "2022-03-01T00:00:00Z",
  "version": "{{_version}}"
}
""", json);
      Assert.AreEqual(_fixedTime, data.LastRecordAt);
      Assert.AreEqual(0, data.BestRecords.Count);
      Assert.AreEqual(_version, data.Version);
    }

    [TestMethod]
    public void TestSaveSingle() {
      var anotherTime = DateTime.Parse("2022-03-02T00:00:00Z");

      var (json, data) = AccuracyRepository.GetPersistentData(_singleRecords, anotherTime);

      Assert.AreEqual($$"""
{
  "bestRecords": [
    { "levelId": "custom_level_000", "type": "Standard", "difficulty": "ExpertPlus", "accuracy": 0.90292, "stars": 5.5 }
  ],
  "lastRecordAt": "2022-03-02T00:00:00Z",
  "version": "{{_version}}"
}
""", json);
      Assert.AreEqual(anotherTime, data.LastRecordAt);
      Assert.AreEqual(1, data.BestRecords.Count);
      Assert.AreEqual(_version, data.Version);
    }

    [TestMethod]
    public void TestSaveCombined() {
      var (json, data) = AccuracyRepository.GetPersistentData(_combinedRecords, _fixedTime);

      Assert.AreEqual($$"""
{
  "bestRecords": [
    { "levelId": "custom_level_111", "type": "Standard", "difficulty": "Expert", "accuracy": 0.92192, "stars": 5.1 },
    { "levelId": "custom_level_111", "type": "Standard", "difficulty": "ExpertPlus", "accuracy": 0.90292, "stars": 6.2 }
  ],
  "lastRecordAt": "2022-03-01T00:00:00Z",
  "version": "{{_version}}"
}
""", json);
      Assert.AreEqual(_fixedTime, data.LastRecordAt);
      Assert.AreEqual(2, data.BestRecords.Count);
      Assert.AreEqual(_version, data.Version);
    }

    [TestMethod]
    public void TestSaveDouble() {
      var (json, data) = AccuracyRepository.GetPersistentData(_doubleRecords, _fixedTime);

      Assert.AreEqual($$"""
{
  "bestRecords": [
    { "levelId": "custom_level_111", "type": "Standard", "difficulty": "Expert", "accuracy": 0.92192, "stars": 5.1 },
    { "levelId": "custom_level_222", "type": "Lawless", "difficulty": "Hard", "accuracy": 0.91, "stars": 4.8 },
    { "levelId": "custom_level_111", "type": "Standard", "difficulty": "ExpertPlus", "accuracy": 0.90292, "stars": 6.2 }
  ],
  "lastRecordAt": "2022-03-01T00:00:00Z",
  "version": "{{_version}}"
}
""", json);
      Assert.AreEqual(_fixedTime, data.LastRecordAt);
      Assert.AreEqual(3, data.BestRecords.Count);
      Assert.AreEqual(_version, data.Version);
    }

    [TestMethod]
    public void TestIdempotency() {
      var (_, data) = AccuracyRepository.GetPersistentData(_doubleRecords, _fixedTime);
      var sorterData = AccuracyRepository.GetSorterData(data.BestRecords);
      (string json, data) = AccuracyRepository.GetPersistentData(sorterData, _fixedTime);

      Assert.AreEqual($$"""
{
  "bestRecords": [
    { "levelId": "custom_level_111", "type": "Standard", "difficulty": "Expert", "accuracy": 0.92192, "stars": 5.1 },
    { "levelId": "custom_level_222", "type": "Lawless", "difficulty": "Hard", "accuracy": 0.91, "stars": 4.8 },
    { "levelId": "custom_level_111", "type": "Standard", "difficulty": "ExpertPlus", "accuracy": 0.90292, "stars": 6.2 }
  ],
  "lastRecordAt": "2022-03-01T00:00:00Z",
  "version": "{{_version}}"
}
""", json);
      Assert.AreEqual(_fixedTime, data.LastRecordAt);
      Assert.AreEqual(3, data.BestRecords.Count);
      Assert.AreEqual(_version, data.Version);
    }
  }
}
