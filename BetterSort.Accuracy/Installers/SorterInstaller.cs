using BetterSort.Accuracy.External;
using BetterSort.Accuracy.Sorter;
using BetterSort.Common.Compatibility;
using BetterSort.Common.Flows;
using Zenject;

namespace BetterSort.Accuracy.Installers {

  public class SorterInstaller : Installer {

    public override void InstallBindings() {
      Container.BindInterfacesAndSelfTo<ScoresaberBest>().AsSingle();
      Container.BindInterfacesAndSelfTo<BeatleaderBest>().AsSingle();
      Container.Bind<UnifiedImporter>().AsSingle();
      Container.BindInterfacesAndSelfTo<AccuracyRepository>().AsSingle();

      // Accuracy sorter
      Container.Bind<AccuracySorter>().AsSingle();
      Container.Bind<UIAwareSorter>().AsSingle();
      Container.Bind<DifficultySelectingSorter>()
        .FromMethod(ctx => new DifficultySelectingSorter(
          ctx.Container.Resolve<UIAwareSorter>(),
          ctx.Container.Resolve<SiraUtil.Logging.SiraLog>(),
          ctx.Container.Resolve<BetterSort.Common.External.ISongSelection>(),
          ctx.Container.Resolve<UIAwareSorter>()))
        .AsSingle();
      Container.BindInterfacesAndSelfTo<FilterSortAdaptor>()
        .FromMethod(ctx => new FilterSortAdaptor(
          ctx.Container.Resolve<DifficultySelectingSorter>(),
          ctx.Container.Resolve<SiraUtil.Logging.SiraLog>()))
        .AsSingle();

      // Star Rating sorter
      Container.Bind<StarRatingSorter>().AsSingle();
      Container.Bind<StarRatingUIAwareSorter>().AsSingle();
      Container.Bind<DifficultySelectingSorter>()
        .WithId("StarRating")
        .FromMethod(ctx => new DifficultySelectingSorter(
          ctx.Container.Resolve<StarRatingUIAwareSorter>(),
          ctx.Container.Resolve<SiraUtil.Logging.SiraLog>(),
          ctx.Container.Resolve<BetterSort.Common.External.ISongSelection>(),
          ctx.Container.Resolve<StarRatingUIAwareSorter>()))
        .AsSingle();
      Container.BindInterfacesAndSelfTo<FilterSortAdaptor>()
        .WithId("StarRating")
        .FromMethod(ctx => new FilterSortAdaptor(
          ctx.Container.ResolveId<DifficultySelectingSorter>("StarRating"),
          ctx.Container.Resolve<SiraUtil.Logging.SiraLog>()))
        .AsSingle();

      Container.BindInterfacesAndSelfTo<SorterEnvironment>().AsSingle().NonLazy();
    }
  }
}
