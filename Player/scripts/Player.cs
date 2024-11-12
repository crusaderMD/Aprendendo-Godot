using Godot;
using System;

public partial class Player : CharacterBody2D
{
    // A direção inicial do jogador, começando olhando para baixo.
    private Vector2 cardinalDirection = Vector2.Down;

    // A direção atual do movimento do jogador (inicialmente sem movimento).
    private Vector2 direction = Vector2.Zero;

    // Enum que define os estados do jogador, 'Idle' para parado e 'Walk' para andando.
    private enum PlayerState { Idle, Walk }
    private PlayerState state = PlayerState.Idle; // O jogador começa no estado "Idle".

    // Propriedade exportada para permitir ajuste da velocidade de movimento no editor.
    [Export]
    public float MoveSpeed { get; set; } = 100f; // Valor padrão da velocidade de movimento do jogador.

    // Referências para os nós de animação e sprite do jogador.
    private AnimationPlayer animationPlayer;
    private Sprite2D sprite;

    // Método chamado quando o nó é instanciado, inicializa as referências de animação e sprite.
    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer"); // Referência para o AnimationPlayer.
        sprite = GetNode<Sprite2D>("Sprite2D"); // Referência para o Sprite2D (imagem do personagem).
    }

    // Método chamado a cada frame. Aqui verificamos a direção do movimento e atualizamos a animação.
    public override void _Process(double delta)
    {
        direction = GetInputDirection(); // Obtém a direção de movimento com base na entrada do jogador.

        // Se o estado ou a direção mudaram, atualizamos a animação.
        if (SetState() || SetDirection())
        {
            UpdateAnimation(); // Atualiza a animação com base no novo estado e direção.
        }
    }

    // Método chamado para movimentação física a cada frame. Calcula e aplica a velocidade do movimento.
    public override void _PhysicsProcess(double delta)
    {
        Velocity = direction * MoveSpeed; // A velocidade é calculada multiplicando a direção pela velocidade.
        MoveAndSlide(); // Move o jogador com base na velocidade e aplica a física.
    }

    // Método que retorna a direção do movimento com base nas teclas pressionadas.
    private Vector2 GetInputDirection()
    {
        // Retorna um vetor com valores entre -1 e 1, dependendo das teclas pressionadas.
        return new Vector2(
            Input.GetActionStrength("right") - Input.GetActionStrength("left"), // Determina o movimento horizontal (esquerda/direita).
            Input.GetActionStrength("down") - Input.GetActionStrength("up") // Determina o movimento vertical (cima/baixo).
        );
    }

    // Método que determina se a direção do personagem mudou e ajusta a direção do sprite.
    public bool SetDirection()
    {
        // Se não houver movimento, não alteramos a direção.
        if (direction == Vector2.Zero)
        {
            return false;
        }

        // A direção é normalizada para garantir que o vetor tenha um comprimento de 1, evitando velocidades diagonais desproporcionais.
        Vector2 normalizedDirection = direction.Normalized();
        Vector2 newDirection = cardinalDirection; // Começa com a direção atual.

        // Se a componente X for maior que a Y, o movimento é predominantemente horizontal.
        if (Mathf.Abs(normalizedDirection.X) > Mathf.Abs(normalizedDirection.Y))
        {
            newDirection = (normalizedDirection.X < 0) ? Vector2.Left : Vector2.Right; // Ajusta para esquerda ou direita.
        }
        else
        {
            newDirection = (normalizedDirection.Y < 0) ? Vector2.Up : Vector2.Down; // Ajusta para cima ou para baixo.
        }

        // Se a direção não mudou, não há necessidade de atualizar.
        if (newDirection == cardinalDirection) return false;

        cardinalDirection = newDirection; // Atualiza a direção do personagem.

        return true;
    }

    // Método que determina se o estado (Idle ou Walk) do personagem mudou.
    public bool SetState()
    {
        PlayerState newState = (direction == Vector2.Zero) ? PlayerState.Idle : PlayerState.Walk; // Se não há movimento, o estado é Idle.

        // Se o estado não mudou, não fazemos nada.
        if (newState == state) return false;

        state = newState; // Atualiza o estado do personagem.

        return true;
    }

    // Método que atualiza a animação com base no estado e direção.
    public void UpdateAnimation()
    {
        // Cria o nome da animação com base no estado e direção. Exemplo: "idle_left" ou "walk_up".
        string animationName = $"{GetStateString()}_{AnimDirectionToString()}";

        // Verifica se a animação existe e, caso exista, a toca.
        if (animationPlayer.HasAnimation(animationName))
        {
            animationPlayer.Play(animationName);
        }
        else
        {
            GD.PrintErr($"Animation not found: \"{animationName}\""); // Se a animação não existir, exibe um erro.
        }
    }

    // Método que retorna o nome do estado como string (em minúsculas).
    private string GetStateString()
    {
        return state.ToString().ToLower(); // Exemplo: "idle" ou "walk".
    }

    // Método que converte a direção cardinal para uma string compatível com a animação.
    public string AnimDirectionToString()
    {
        if (cardinalDirection == Vector2.Down) return "down"; //Para baixo
        if (cardinalDirection == Vector2.Up) return "up"; //Para cima
        if (cardinalDirection == Vector2.Left) return "left"; //Para esquerda
        return "right"; // Para direita.
    }
}
