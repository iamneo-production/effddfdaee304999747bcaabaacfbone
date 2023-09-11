using System.Collections.Generic;
using System.Linq;
using dotnetapp.Controllers;
using dotnetapp.Exceptions;
using dotnetapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Reflection;

namespace dotnetapp.Tests
{
    [TestFixture]
public class RideSharingTests
{
    private Type programType = typeof(SlotController);

    private DbContextOptions<RideSharingDbContext> _dbContextOptions;

    [SetUp]
    public void Setup()
    {
        _dbContextOptions = new DbContextOptionsBuilder<RideSharingDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        using (var dbContext = new RideSharingDbContext(_dbContextOptions))
        {
            // Add test data to the in-memory database
            var ride = new Ride
            {
                RideID = 1,
                DepartureLocation = "Location A",
                Destination = "Location B",
                DateTime = DateTime.Parse("2023-08-30"),
                MaximumCapacity = 4
            };

            dbContext.Rides.Add(ride);
            dbContext.SaveChanges();
        }
    }

    [TearDown]
    public void TearDown()
    {
        using (var dbContext = new RideSharingDbContext(_dbContextOptions))
        {
            // Clear the in-memory database after each test
            dbContext.Database.EnsureDeleted();
        }
    }

    private MethodInfo GetMethodInfo(object controller, string methodName, bool isHttpPost)
        {
            Type controllerType = controller.GetType();
            MethodInfo method = controllerType.GetMethod(methodName);

            if (method == null)
            {
                Assert.Fail($"{methodName} method not found.");
            }

            int parametersLength = isHttpPost ? 3 : 1;

            if (method.GetParameters().Length != parametersLength)
            {
                Assert.Fail($"Invalid number of parameters for {methodName} method.");
            }

            if (isHttpPost)
            {
                if (method.GetParameters()[0].ParameterType != typeof(int) ||
                    method.GetParameters()[1].ParameterType != typeof(Commuter))
                {
                    Assert.Fail($"Invalid parameter types for {methodName} method.");
                }
            }
            else
            {
                if (method.GetParameters()[0].ParameterType != typeof(int))
                {
                    Assert.Fail($"Invalid parameter types for {methodName} method.");
                }
            }

            return method;
        }
        private MethodInfo GetJoinRideMethodInfo(SlotController controller)
        {
            Type controllerType = controller.GetType();
            MethodInfo joinRideMethod = controllerType.GetMethod("JoinRide", new[] { typeof(int), typeof(Commuter) });

            Assert.IsNotNull(joinRideMethod, "JoinRide method not found.");
            return joinRideMethod;
        }


    [Test]
        public void JoinRide_ValidCommuter_JoinsSuccessfully()
        {
            // Arrange
            using (var dbContext = new RideSharingDbContext(_dbContextOptions))
            {
                var slotController = new SlotController(dbContext);
                var commuter = new Commuter
                {
                    Name = "John Doe",
                    Email = "johndoe@example.com",
                    Phone = "1234567890"
                };

                MethodInfo joinRideMethod = GetJoinRideMethodInfo(slotController);

                // Act
                var result = joinRideMethod.Invoke(slotController, new object[] { 1, commuter }) as RedirectToActionResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual("Details", result.ActionName);
                Assert.AreEqual("Ride", result.ControllerName);

                var ride = dbContext.Rides.Include(r => r.Commuters).FirstOrDefault(r => r.RideID == 1);
                Assert.IsNotNull(ride);
                Assert.AreEqual(1, ride.Commuters.Count);
                Assert.AreEqual(4, ride.MaximumCapacity);
            }
        }


    [Test]
        public void JoinRide_ValidCommuter_JoinsSuccessfully2()
        {
            // Arrange
            using (var dbContext = new RideSharingDbContext(_dbContextOptions))
            {
                var slotController = new SlotController(dbContext);
                var commuter = new Commuter
                {
                    Name = "John Doe",
                    Email = "johndoe@example.com",
                    Phone = "1234567890"
                };

                MethodInfo joinRideMethod = GetJoinRideMethodInfo(slotController);

                // Act
                var result = joinRideMethod.Invoke(slotController, new object[] { 1, commuter }) as RedirectToActionResult;

                var ride = dbContext.Rides.Include(r => r.Commuters).FirstOrDefault(r => r.RideID == 1);
                Assert.IsNotNull(ride);
                Assert.AreEqual(1, ride.Commuters.Count);
            }
        }


    [Test]
    public void JoinRide_ValidCommuter_JoinsSuccessfully3()
    {
        using (var dbContext = new RideSharingDbContext(_dbContextOptions))
        {
            // Arrange
            var slotController = new SlotController(dbContext);
            var commuter = new Commuter
            {
                Name = "John Doe",
                Email = "johndoe@example.com",
                Phone = "1234567890"
            };

            // Act
            MethodInfo joinRideMethod = GetJoinRideMethodInfo(slotController);      

                // Act
            var result = joinRideMethod.Invoke(slotController, new object[] { 1, commuter }) as RedirectToActionResult;            
            var ride = dbContext.Rides.Include(r => r.Commuters).FirstOrDefault(r => r.RideID == 1);

            Assert.AreEqual(4, ride.MaximumCapacity);
        }
    }




    [Test]
    public void JoinRide_ValidCommuter_JoinsSuccessfully1()
    {
        using (var dbContext = new RideSharingDbContext(_dbContextOptions))
        {
            // Arrange
            var slotController = new SlotController(dbContext);
            var commuter = new Commuter
            {
                Name = "John Doe",
                Email = "johndoe@example.com",
                Phone = "1234567890"
            };

            // Act
MethodInfo joinRideMethod = GetJoinRideMethodInfo(slotController);

                // Act
                var result = joinRideMethod.Invoke(slotController, new object[] { 1, commuter }) as RedirectToActionResult;
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Details", result.ActionName);
            Assert.AreEqual("Ride", result.ControllerName);
        }
    }



    [Test]
    public void JoinRide_RideNotFound_ReturnsNotFoundResult()
    {
        using (var dbContext = new RideSharingDbContext(_dbContextOptions))
        {
            // Arrange
            var slotController = new SlotController(dbContext);
            var commuter = new Commuter
            {
                Name = "John Doe",
                Email = "johndoe@example.com",
                Phone = "1234567890"
            };

            // Act
            // var result = slotController.JoinRide(2, commuter) as NotFoundResult;
            MethodInfo joinRideMethod = GetJoinRideMethodInfo(slotController);

                // Act
            var result = joinRideMethod.Invoke(slotController, new object[] { 2, commuter }) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
    
    [Test]
        public void JoinRide_MaximumCapacityReached_ThrowsException()
        {
            // Arrange
            using (var dbContext = new RideSharingDbContext(_dbContextOptions))
            {
                var slotController = new SlotController(dbContext);
                var commuter1 = new Commuter
                {
                    Name = "John Doe",
                    Email = "johndoe@example.com",
                    Phone = "1234567890"
                };

                var commuter2 = new Commuter
                {
                    Name = "Jane Smith",
                    Email = "janesmith@example.com",
                    Phone = "9876543210"
                };

                var ride = dbContext.Rides.Include(r => r.Commuters).FirstOrDefault(r => r.RideID == 1);
                ride.Commuters.Add(commuter1);
                ride.Commuters.Add(commuter2);
                ride.MaximumCapacity = 2;

                dbContext.SaveChanges();

                var commuter3 = new Commuter
                {
                    Name = "Alice Johnson",
                    Email = "alicejohnson@example.com",
                    Phone = "5555555555"
                };

                MethodInfo joinRideMethod = GetJoinRideMethodInfo(slotController);

                // Act & Assert
                try
                {
                    joinRideMethod.Invoke(slotController, new object[] { 1, commuter3 });
                    Assert.Fail("Expected RideSharingException, but no exception was thrown.");
                }
                catch (TargetInvocationException ex)
                {
                    // Unwrap the inner exception and check its type
                    Assert.IsTrue(ex.InnerException is RideSharingException, "Expected RideSharingException, but got: " + ex.InnerException.GetType().Name);
                }
            }
        }


    [Test]
        public void JoinRide_MaximumCapacityReached_ThrowsExceptionwith_message()
        {
            // Arrange
            using (var dbContext = new RideSharingDbContext(_dbContextOptions))
            {
                var slotController = new SlotController(dbContext);
                var commuter1 = new Commuter
                {
                    Name = "John Doe",
                    Email = "johndoe@example.com",
                    Phone = "1234567890"
                };

                var commuter2 = new Commuter
                {
                    Name = "Jane Smith",
                    Email = "janesmith@example.com",
                    Phone = "9876543210"
                };

                var ride = dbContext.Rides.Include(r => r.Commuters).FirstOrDefault(r => r.RideID == 1);
                ride.Commuters.Add(commuter1);
                ride.Commuters.Add(commuter2);
                ride.MaximumCapacity = 2;

                dbContext.SaveChanges();

                var commuter3 = new Commuter
                {
                    Name = "Alice Johnson",
                    Email = "alicejohnson@example.com",
                    Phone = "5555555555"
                };

                MethodInfo joinRideMethod = GetJoinRideMethodInfo(slotController);

                // Act & Assert
                try
                {
                    joinRideMethod.Invoke(slotController, new object[] { 1, commuter3 });
                    Assert.Fail("Expected RideSharingException, but no exception was thrown.");
                }
                catch (TargetInvocationException ex)
                {
                    // Unwrap the inner exception and check its type
                    Assert.IsTrue(ex.InnerException is RideSharingException, "Expected RideSharingException, but got: " + ex.InnerException.GetType().Name);
                    Assert.AreEqual("Maximum capacity reached", ex.InnerException.Message);
                }
            }
        }



        [Test]
        public void Commuter_Ride_ReturnsExpectedValue()
        {
            // Arrange
            Ride expectedRide = new Ride { RideID = 2 };
            Commuter commuter = new Commuter { Ride = expectedRide };

            // Assert
            Assert.AreEqual(expectedRide, commuter.Ride);
        }

       

   
}

}