using BetterSort.Accuracy.External;
using BetterSort.Common.Models;
using System.Collections.Generic;

namespace BetterSort.Accuracy.Sorter {

  internal class LevelStarRatingComparer(SorterData records) : IComparer<ILevelPreview> {
    private readonly SorterData _records = records;

    public Dictionary<ILevelPreview, LevelRecord> LevelMap { get; set; } = [];

    public int Compare(ILevelPreview a, ILevelPreview b) {
      if (_records == null) {
        return 0;
      }

      if (LevelMap.TryGetValue(a, out var bestA)) {
        if (LevelMap.TryGetValue(b, out var bestB)) {
          int descending = bestB.Stars.CompareTo(bestA.Stars);
          return descending;
        }
        return -1;
      }
      else {
        if (LevelMap.ContainsKey(b)) {
          return 1;
        }
        return 0;
      }
    }

    public List<ILevelPreview> Inflate(ILevelPreview preview) {
      var result = new List<ILevelPreview>();
      if (_records.TryGetValue(preview.LevelId, out var levelRecords) && levelRecords.Count > 0) {
        foreach (var record in levelRecords) {
          var (type, difficulty) = record.Key;
          var scoreRecord = record.Value;
          var clone = preview.Clone();
          LevelMap.Add(clone, new(type, difficulty, scoreRecord.Accuracy, scoreRecord.Stars));
          result.Add(clone);
        }
      }
      else {
        result.Add(preview);
      }
      return result;
    }
  }
}
