using Parcel.Neo.Base.DataTypes;
using Parcel.Physics.Simulation;

namespace Parcel.Simulation
{
    /// <summary>
    /// Provides configurations for overall behvaior of the physical world
    /// TODO: Provide logging/output options
    /// </summary>
    public class PhysicsWorldConfiguration
    {
        /// <summary>
        /// Gravity on the global scale
        /// </summary>
        public Vector3D GlobalGravity { get; set; }
        public Vector3D GlobalWindow { get; set; }
        public double LinearDamping { get; set; }
        public double AngularDamping {  get; set; }
    }
    /// <summary>
    /// Defines a tangible physics body (rigid body).
    /// </summary>
    public class RigidBody
    {

    }

    /// <summary>
    /// Provides static helper methods for kicking starting the simulation
    /// </summary>
    public static class PhysicsHelper
    {
        #region Simulation Creation Helper
        /// <summary>
        /// Create a rigid body to be used in the simulation
        /// </summary>
        public static RigidBody CreateBody() => new();
        /// <summary>
        /// Start the simulation with a set of configurations
        /// </summary>
        public static PhysicsWorld StartSimulation(double timeStep, double duration, PhysicsWorldConfiguration options, params RigidBody[] bodies)
        {
            PhysicsWorld world = new(options);
            world.AddBody(world);
            world.Simulate(timeStep, duration);
            return world;
        }
        #endregion
    }
}
