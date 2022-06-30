namespace Indra.Data {
  public partial class BuiltInCommand {
    public partial class Get : Command<IModel>, ICommand.IReturn<IModel> {

      /// <summary>
      /// The value returned from the executed get command.
      /// </summary>
      public IModel Return {
        get;
        private set;
      } = null;
    }
  }
}