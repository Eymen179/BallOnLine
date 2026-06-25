public interface IInteractable
{
    // Bu arayüzü kullanan her obje, içine bir top (BallController) aldýðýnda ne yapacaðýný bilmek zorundadýr.
    void Interact(BallController ball);
}