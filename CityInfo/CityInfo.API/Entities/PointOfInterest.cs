using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {
        /// <summary>
        /// Similarly here, as we did with 'City' class,
        /// by defining this overloaded constructor,
        /// we convey to any reader/developer that we always
        /// want this 'PointOfInterest' class to have a name
        /// </summary>
        public PointOfInterest(string name)
        {
            Name = name;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        // This is considered a Navigation property and a relationship is created between
        // the current 'PointOfInterest' entry and the parent 'City' by the current Database provider.
        // Relationships discovered by convention will always target the primary key of the principal entity
        // which in this case is the 'Id' of the 'City' principal entity.
        // B) If we don't want to follow the "Convention-based" for the 'ForeignKey' property:
        // The 'ForeignKey' annotation can be used on the Navigation property
        [ForeignKey("CityId")]
        public City? City { get; set; }

        // A) If we want to follow the "Convention-based" for the 'ForeignKey' property:
        // It is not required to explicitly define the 'ForeignKey' property in the dependent class,
        // which here is the 'PointOfInterest' class. Although it is recommended to do so, for clarity.
        // According to the "convention-based" approach, the 'ForeignKey' will be named based on the Navigation preperty's
        // class name (which in this case is 'City') followed by the term 'Id', resulting into this 'CityId' property.
        public int CityId { get; set; }
    }
}
