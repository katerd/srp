using System.Collections.Generic;
using FluentAssertions;
using Moq;
using SrpTask;
using Xunit;

namespace Tests
{
    public class RpgPlayerTests
    {
        [Fact]
        public void PickUpItem_ThatCanBePickedUp_ItIsAddedToTheInventory()
        {
            // Arrange
            var engine = new Mock<IGameEngine>();
            var player = new RpgPlayer(engine.Object);
            var item = ItemBuilder.Build.AnItem();

            player.Inventory.Should().BeEmpty();

            // Act
            player.PickUpItem(item);

            // Assert
            player.Inventory.Should().Contain(item);
        }

        [Fact]
        public void PickUpItem_ThatGivesHealth_HealthIsIncreaseAndItemIsNotAddedToInventory()
        {
            // Arrange
            var engine = new Mock<IGameEngine>();
            var player = new RpgPlayer(engine.Object)
            {
                MaxHealth = 100,
                Health = 10
            };

            var healthPotion = 
                ItemBuilder
                .Build
                .WithHeal(100)
                .AnItem();

            // Act
            player.PickUpItem(healthPotion);

            // Assert
            player.Inventory.Should().BeEmpty();
            player.Health.Should().Be(100);
        }

        [Fact]
        public void PickUpItem_ThatGivesHealth_HealthDoesNotExceedMaxHealth()
        {
            // Arrange
            var engine = new Mock<IGameEngine>();
            var player = new RpgPlayer(engine.Object)
            {
                MaxHealth = 50,
                Health = 10
            };

            var healthPotion =
                ItemBuilder
                .Build
                .WithHeal(100)
                .AnItem();

            // Act
            player.PickUpItem(healthPotion);

            // Assert
            player.Inventory.Should().BeEmpty();
            player.Health.Should().Be(50);
        }

        [Fact]
        public void PickUpItem_ThatIsRare_ASpecialEffectShouldBePlayed()
        {
            // Arrange
            var engine = new Mock<IGameEngine>();
            var player = new RpgPlayer(engine.Object);
            var rareItem = ItemBuilder.Build.IsRare(true).AnItem();

            engine.Setup(x => x.PlaySpecialEffect("cool_swirly_particles")).Verifiable();

            // Act
            player.PickUpItem(rareItem);

            // Assert
            engine.VerifyAll();
        }

        [Fact]
        public void PickUpItem_ThatIsUnique_ItShouldNotBePickedUpIfThePlayerAlreadyHasIt()
        {
            // Arrange
            var engine = new Mock<IGameEngine>();
            var player = new RpgPlayer(engine.Object);
            player.PickUpItem(ItemBuilder.Build.WithId(100).AnItem());

            var uniqueItem = ItemBuilder
                .Build
                .WithId(100)
                .IsUnique(true)
                .AnItem();

            // Act
            var result = player.PickUpItem(uniqueItem);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void PickUpItem_ThatDoesMoreThan500Healing_AnExtraGreenSwirlyEffectOccurs()
        {
            // Arrange
            var engine = new Mock<IGameEngine>();
            var player = new RpgPlayer(engine.Object);
            var xPotion = ItemBuilder.Build.WithHeal(501).AnItem();

            engine.Setup(x => x.PlaySpecialEffect("green_swirly")).Verifiable();

            // Act
            player.PickUpItem(xPotion);

            // Assert
            engine.VerifyAll();
        }

        [Fact]
        public void PickUpItem_ThatGivesArmour_ThePlayersArmourValueShouldBeIncreased()
        {
            // Arrange
            var engine = new Mock<IGameEngine>();
            var player = new RpgPlayer(engine.Object);
            player.Armour.Should().Be(0);

            var armour = ItemBuilder.Build.WithArmour(100).AnItem();

            // Act
            player.PickUpItem(armour);

            // Assert
            player.Armour.Should().Be(100);
        }

        [Fact]
        public void PickUpItem_ThatIsTooHeavy_TheItemIsNotPickedUp()
        {
            // Arrange
            var engine = new Mock<IGameEngine>();
            var player = new RpgPlayer(engine.Object);
            var heavyItem = ItemBuilder.Build.WithWeight(player.CarryingCapacity + 1).AnItem();

            // Act
            var result = player.PickUpItem(heavyItem);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TakeDamage_WithNoArmour_HealthIsReducedAndParticleEffectIsShown()
        {
            // Arrange
            var engine = new Mock<IGameEngine>();
            var player = new RpgPlayer(engine.Object);
            engine.Setup(x => x.PlaySpecialEffect("lots_of_gore")).Verifiable();
            player.Health = 200;

            // Act
            player.TakeDamage(100);

            // Assert
            player.Health.Should().Be(100);
            engine.VerifyAll();
            
        }

        [Fact]
        public void TakeDamage_With50Armour_DamageIsReducedBy50AndParticleEffectIsShown()
        {
            // Arrange
            var engine = new Mock<IGameEngine>();
            var player = new RpgPlayer(engine.Object);
            engine.Setup(x => x.PlaySpecialEffect("lots_of_gore")).Verifiable();
            player.PickUpItem(ItemBuilder.Build.WithArmour(50).AnItem());
            player.Health = 200;

            // Act
            player.TakeDamage(100);

            // Assert
            player.Health.Should().Be(150);
        }

        [Fact]
        public void TakeDamage_WithMoreArmourThanDamage_NoDamageIsDealtAndParryEffectIsPlayed()
        {
            // Arrange
            var engine = new Mock<IGameEngine>();
            var player = new RpgPlayer(engine.Object);
            engine.Setup(x => x.PlaySpecialEffect("parry")).Verifiable();
            player.PickUpItem(ItemBuilder.Build.WithArmour(2000).AnItem());
            player.Health = 200;

            // Act
            player.TakeDamage(100);

            // Assert
            player.Health.Should().Be(200);
            engine.VerifyAll();
        }

        [Fact]
        public void UseItem_StinkBomb_AllEnemiesNearThePlayerAreDamaged()
        {
            // Arrange
            var engine = new Mock<IGameEngine>();
            var player = new RpgPlayer(engine.Object);
            var enemy = new Mock<IEnemy>();

            var item = ItemBuilder.Build.WithName("Stink Bomb").AnItem();
            engine.Setup(x => x.GetEnemiesNear(player))
                .Returns(new List<IEnemy>
                {
                    enemy.Object
                });

            // Act
            player.UseItem(item);

            // Assert
            enemy.Verify(x => x.TakeDamage(100));
        }
    }
}
