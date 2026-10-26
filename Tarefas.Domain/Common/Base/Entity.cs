using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Domain.Common.Interfaces;

namespace Tarefas.Domain.Common.Base
{
    public abstract class Entity
    {

        //Usar protected set para que apenas a própria clase ou claasses derivadas possam alterar o Id,
        //reforçando a imutabilidade do Id após a criação da entidade.
        public Guid Id {get; protected set; }
        public DateTime DataCriacao { get; protected set; }
        public DateTime? DataAtualizacao { get; protected set; }

        //Construtor protegido: Entidades devem ser criadas via um Construtor ou Factory
        //e não instanciadas de forma simples
        protected Entity()
        {
            //Atribui um novo Guid no momento da criação  
            Id = Guid.NewGuid();
            DataCriacao = DateTime.UtcNow;
        }

        protected void SetDataAtualizacao() => DataAtualizacao = DateTime.UtcNow;

        //Caso queira um construtor que receba o Id
        protected Entity(Guid id)
        {
            Id = id;
            DataCriacao = DateTime.UtcNow;
        }
        
        //Sobrescrever Equals e GetHashCode é crucial para compara Entidades baseado apenas na sua identidade (ID)
        public override bool Equals(object? obj)
        {
            //Pede para se é realmente uma entidade, caso não seja, retorna false e estará disponível na variável other
            if (obj is not Entity other) return false;

            //Estou comparando um objeto com ele mesmo, se sim retorna true
            if(ReferenceEquals(this, other)) return true; 

            //Se o Id ainda não foi persistido (é Guid.Empty), compara por referência
            return Id.Equals(other.Id);
        }

        //Esse método gera um número inteiro baseado no Id
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator == (Entity left, Entity right)
        {
            if(ReferenceEquals(left, null)) 
                return ReferenceEquals(right, null);

            return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);

        }

        //Cria uma lista de eventos do domínio, e cada evento que for adicionado
        //a ela deve implementar a interface IDomainEvent, que é uma marcação para eventos do domínio.
        private readonly List<IDomainEvent> _domainEvents = new();

        //Expõe os eventos e domínio como uma coleção somente leitura
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        //Método protegido par adicionar um evento de domíncio
        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        //Método protegido para remover um evento de domínio
        public void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        //Método protegido para limpar todos os eventos de domínio
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}

