# BetterSort

This repository contains additional beat saber sort plugins can be used with [BetterSongList](https://github.com/kinsi55/BeatSaber_BetterSongList).
All plugins here require BetterSongList.

## BetterSort.LastPlayed

<img src="docs/preview.webp" alt="last played sorter screen" width="300"/>

This plugin records your last played date and sorts songs accordingly.

## BetterSort.Accuracy

<img src="docs/accuracy-preview.webp" alt="accuracy sorter screen" height="300"/>

This plugin provides two sorters:

- **Accuracy**: Sorts maps by difficulty according to your best accuracy on each difficulty.
- **Star Rating**: Sorts maps by difficulty according to their star rating (from ScoreSaber/BeatLeader). Each difficulty becomes its own list entry, sorted by star rating in descending order.

Both sorters split maps by difficulty, allowing you to see all difficulties of a map as separate entries.

## Installation

You can use [ModAssistant](https://github.com/Assistant/ModAssistant/releases/latest) for installation, which I fully support.

If plugins isn't available on ModAssistant or new features are added later, you can [download plugins manually](https://github.com/nanikit/BetterSort/releases).

When manually downloading mods, please be careful of the mod's dependencies. Currently BS_Utils.dll, SiraUtil.dll, and BetterSongList.dll are required.

## Usage

Simply click the left-bottom sort button in the song select scene, and choose either 'Last played', 'Accuracy', or 'Star Rating'.

## Q&A

- Q: Just installed, it doesn't sort at all.

  A: For LastPlayed, immediately after installation, there's no play history recorded. So, play some songs first.<br />

  For Accuracy and Star Rating, the plugin gathers your scores from ScoreSaber and BeatLeader upon first launch. This process may take a while depending on your score history.

- Q: Reverse sort doesn't work.<br />
  A: I intentionally didn't support reverse sorting. Currently BetterSongList doesn't remember the sort direction of each sorter, which caused confusion for me while playing.
