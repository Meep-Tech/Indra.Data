using Meep.Tech.Data;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Indra.Data {
  public class User : Model<User>.WithComponents, ICached {

    [AutoBuild, NotNull, Required]
    public string UniqueName {
      get;
      private set;
    }

    [AutoBuild, NotNull, Required, JsonIgnore]
    public string PasswordHash {
      get;
      private set;
    }

    public string Id { get; private set; }
    string IUnique.Id { get => Id; set => Id = value; }
  }
}
