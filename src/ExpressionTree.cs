using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    class ExpressionNode
    {
        public string value;
        public ExpressionNode left, right, parent;

        //Node constructor
        public ExpressionNode(string value, ExpressionNode parent = null)
        {
            this.value = value;
            left = null;
            right = null;
            this.parent = parent;
        }
    }

    class ExpressionTree
    {
        private ExpressionNode root;

        public ExpressionTree()
        {
            root = null;
        }
        
        public ExpressionNode GetRoot()
        {
            return root;
        }

        /// <summary>
        /// Insert node with a value after another node in tree.
        /// </summary>
        /// <param name="value">Value of a node</param>
        /// <param name="parent">Parent node. Can be null to set as root.</param>
        /// <returns>Newly created node or null, if creation failed.</returns>
        public ExpressionNode Insert(string value, ExpressionNode parent)
        {
            ExpressionNode node = new ExpressionNode(value, parent);
            if (root == null)
            {
                root = node;

            }
            else if (parent.left == null)
            {
                parent.left = node;
            }
            else if (parent.right == null)
            {
                parent.right = node;
            }
            else
            {
                return null;
            }

            return node;
        }

        /// <summary>
        /// Remove a node from a tree. This will remove the whole subtree, if removed node had any.
        /// </summary>
        /// <param name="node">Node to remove</param>
        public void Remove(ExpressionNode node)
        {
            if(node.parent != null)
            {
                if(node.parent.left == node)
                {
                    node.parent.left = null;
                }
                else if(node.parent.right == node)
                {
                    node.parent.right = null;
                }
            }
        }
    }
}
