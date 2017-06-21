using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SrpTask.Game;

namespace SrpTask.GameTests
{
    [TestFixture]
    public class RpgPlayerTests
    {
        public RpgPlayer Player { get; set; }

        public Mock<IGameEngine> Engine { get; set; }

        [SetUp]
        public void Setup()
        {
            Engine = new Mock<IGameEngine>();
            Player = new RpgPlayer(Engine.Object);
        }

        [Test]
        public void PickUpItem_ThatCanBePickedUp_ItIsAddedToTheInventory()
        {
            // Arrange
            var item = ItemBuilder.Build.AnItem();

            Player.Inventory.Should().BeEmpty();

            // Act
            Player.PickUpItem(item);

            // Assert
            Player.Inventory.Should().Contain(item);
        }

        [Test]
        public void PickUpItem_ThatGivesHealth_HealthIsIncreaseAndItemIsNotAddedToInventory()
        {
            // Arrange
            Player.MaxHealth = 100;
            Player.Health = 10;

            var healthPotion = 
                ItemBuilder
                .Build
                .WithHeal(100)
                .AnItem();

            // Act
            Player.PickUpItem(healthPotion);

            // Assert
            Player.Inventory.Should().BeEmpty();
            Player.Health.Should().Be(100);
        }

        [Test]
        public void PickUpItem_ThatGivesHealth_HealthDoesNotExceedMaxHealth()
        {
            // Arrange
            Player.MaxHealth = 50;
            Player.Health = 10;

            var healthPotion =
                ItemBuilder
                .Build
                .WithHeal(100)
                .AnItem();

            // Act
            Player.PickUpItem(healthPotion);

            // Assert
            Player.Inventory.Should().BeEmpty();
            Player.Health.Should().Be(50);
        }

        [Test]
        public void PickUpItem_ThatIsRare_ASpecialEffectShouldBePlayed()
        {
            // Arrange
            var rareItem = ItemBuilder.Build.IsRare(true).AnItem();

            Engine.Setup(x => x.PlaySpecialEffect("cool_swirly_particles")).Verifiable();

            // Act
            Player.PickUpItem(rareItem);

            // Assert
            Engine.VerifyAll();
        }

        [Test]
        public void PickUpItem_ThatIsUnique_ItShouldNotBePickedUpIfThePlayerAlreadyHasIt()
        {
            // Arrange
            Player.PickUpItem(ItemBuilder.Build.WithId(100).AnItem());

            var uniqueItem = ItemBuilder
                .Build
                .WithId(100)
                .IsUnique(true)
                .AnItem();

            // Act
            var result = Player.PickUpItem(uniqueItem);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void PickUpItem_ThatDoesMoreThan500Healing_AnExtraGreenSwirlyEffectOccurs()
        {
            // Arrange
            var xPotion = ItemBuilder.Build.WithHeal(501).AnItem();

            Engine.Setup(x => x.PlaySpecialEffect("green_swirly")).Verifiable();

            // Act
            Player.PickUpItem(xPotion);

            // Assert
            Engine.VerifyAll();
        }

        [Test]
        public void PickUpItem_ThatGivesArmour_ThePlayersArmourValueShouldBeIncreased()
        {
            // Arrange
            Player.Armour.Should().Be(0);

            var armour = ItemBuilder.Build.WithArmour(100).AnItem();

            // Act
            Player.PickUpItem(armour);

            // Assert
            Player.Armour.Should().Be(100);
        }

        [Test]
        public void PickUpItem_ThatIsTooHeavy_TheItemIsNotPickedUp()
        {
            // Arrange
            var heavyItem = ItemBuilder.Build.WithWeight(Player.CarryingCapacity + 1).AnItem();

            // Act
            var result = Player.PickUpItem(heavyItem);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void TakeDamage_WithNoArmour_HealthIsReducedAndParticleEffectIsShown()
        {
            // Arrange
            Engine.Setup(x => x.PlaySpecialEffect("lots_of_gore")).Verifiable();
            Player.Health = 200;

            // Act
            Player.TakeDamage(100);

            // Assert
            Player.Health.Should().Be(100);
            Engine.VerifyAll();
            
        }

        [Test]
        public void TakeDamage_With50Armour_DamageIsReducedBy50AndParticleEffectIsShown()
        {
            // Arrange
            Engine.Setup(x => x.PlaySpecialEffect("lots_of_gore")).Verifiable();
            Player.PickUpItem(ItemBuilder.Build.WithArmour(50).AnItem());
            Player.Health = 200;

            // Act
            Player.TakeDamage(100);

            // Assert
            Player.Health.Should().Be(150);
        }

        [Test]
        public void TakeDamage_WithMoreArmourThanDamage_NoDamageIsDealtAndParryEffectIsPlayed()
        {
            // Arrange
            Engine.Setup(x => x.PlaySpecialEffect("parry")).Verifiable();
            Player.PickUpItem(ItemBuilder.Build.WithArmour(2000).AnItem());
            Player.Health = 200;

            // Act
            Player.TakeDamage(100);

            // Assert
            Player.Health.Should().Be(200);
            Engine.VerifyAll();
        }

        [Test]
        public void UseItem_StinkBomb_AllEnemiesNearThePlayerAreDamaged()
        {
            // Arrange
            var enemy = new Mock<IEnemy>();

            var item = ItemBuilder.Build.WithName("Stink Bomb").AnItem();
            Engine.Setup(x => x.GetEnemiesNear(Player))
                .Returns(new List<IEnemy>
                {
                    enemy.Object
                });

            // Act
            Player.UseItem(item);

            // Assert
            enemy.Verify(x => x.TakeDamage(100));
        }

        [Test]
        public void PickUpItem_ThatIsRareAndUnique_BlueSwirlyEffectOccurs()
        {
            // Arrange
            var superRareItem =
                ItemBuilder.Build.IsRare(true).IsUnique(true).AnItem();

            Engine.Setup(x => x.PlaySpecialEffect("blue_swirly")).Verifiable();

            // Act
            Player.PickUpItem(superRareItem);

            // Assert
            Engine.VerifyAll();
        }
    }
}