using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR_1_
{
    class Rule
    {
        public Rule()
        {
            ahead = new List<char>();
            ptr = 0;
            id = count;
            count++;
        }
        public Rule(String left, String right)
        {
            ahead = new List<char>();
            ptr = 0;
            id = count;
            count++;
            Left = left;
            Right = right;

        }
        public static int count = 0;

        public int id;

        public String show;
        public String Show
        {
            private set
            {

            }
            get
            {
                return show;
            }
        }

        private String left;
        public String Left
        {
            set
            {
                left = value;
                show = Left + "->" + Right;
            }
            get
            {
                return left;
            }
        }

        private String right;
        public String Right
        {
            set
            {
                right = value;
                show = Left + "->" + Right;
            }
            get
            {
                return right;
            }
        }

        public String Ptr
        {
            get
            {
                if(ptr < Right.Length)
                {
                    return Right[ptr].ToString();
                }
                return null;
                
            }
            private set
            {

            }
        }

        public char this[int index]
        {
            get { return Right[index]; }
        }

        public List<char> ahead;
        public int ptr;

        /// <summary>
        /// 返回一个当前文法的复制（只复制文法的左右两边字符）
        /// </summary>
        /// <returns></returns>
        public Rule Copy()
        {
            Rule copy = new Rule();
            Rule.count--;
            copy.id = this.id;
            copy.Left = this.Left;
            copy.Right = this.Right;
            return copy;
        }

        /// <summary>
        /// 返回一个当前文法的深复制（复制文法的左右两边字符，预测符，点号（ptr））
        /// </summary>
        /// <returns></returns>
        public Rule Clone()
        {
            Rule rule = Copy();
            foreach(char c in this.ahead)
            {
                rule.ahead.Add(c);
            }
            rule.ptr = this.ptr;
            return rule;
        }

        /// <summary>
        /// 判断当前文法是否需要规约（Reduce）
        /// </summary>
        /// <returns></returns>
        public bool IsReduce()
        {
            if(ptr > Right.Length - 1)
            {
                return true;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if(obj is Rule)
            {
                if (((Rule)obj).Show.Equals(this.Show) && ((Rule)obj).ptr == this.ptr)
                {
                    if (((Rule)obj).ahead.Count != this.ahead.Count)
                    {
                        return false;
                    }
                    ((Rule)obj).ahead.Sort();
                    this.ahead.Sort();
                    for (int i = 0; i < this.ahead.Count; i++)
                    {
                        if (((Rule)obj).ahead[i] != this.ahead[i])
                        {
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + Show.GetHashCode();
        }

        public override string ToString()
        {
            String STR = "";
            foreach(char c in ahead)
            {
                STR += " ";
                STR += c;
            }
            return id + ":" + Show + " ," + STR;
        }
    }

    class RuleSet
    {
        public List<Rule> list;

        public static int count = 0;

        public int id;

        public RuleSet()
        {
            id = count;
            count++;
            list = new List<Rule>();
        }

        public void Add(Rule rule)
        {
            list.Add(rule);
        }

        public Rule this[int index]
        {
            get { return list[index]; }
            set { }
        }

        public override bool Equals(object obj)
        {
            if (obj is RuleSet)
            {
                RuleSet objrs = (RuleSet)obj;
                if(objrs.list.Count == this.list.Count)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if(!objrs.list[i].Equals(this.list[i]))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + id;
        }
    }

    class LR
    {
        private List<Rule> rules;               //输入的文法列表
        private List<String> nonterminals;      //非终结符列表
        private List<RuleSet> state;            //状态机中已有状态的列表
        private Dictionary<int, Dictionary<String, String>> table;  //LR1预测表 table[状态的ID][预测符] = 动作

        public LR(List<Rule> rules)
        {
            this.rules = rules;
            table = new Dictionary<int, Dictionary<String, String>>();
            state = new List<RuleSet>();
            nonterminals = new List<String>();
            foreach (Rule rule in rules)
            {
                if (!nonterminals.Contains(rule.Left))
                {
                    nonterminals.Add(rule.Left);
                }
            }
        }

        /// <summary>
        /// 通过ID查找导入文法列表中的对应文法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Rule GetRuleByID(int id)
        {
            foreach(Rule rule in rules)
            {
                if(rule.id == id)
                {
                    return rule;
                }
            }
            return null;
        }

        /// <summary>
        /// 对一个初始状态的状态进行补充
        /// </summary>
        /// <param name="ruleSet"></param>
        private void FillSet(RuleSet ruleSet)
        {
            List<Rule> newAdd = new List<Rule>();
            bool tag = true;
            //循环，知道当前状态集不再变化
            while (tag)
            {
                tag = false;
                foreach (Rule rule in ruleSet.list)
                {
                    if (nonterminals.Contains(rule.Ptr))
                    {
                        //向状态中补充文法
                        foreach (Rule r in rules)
                        {                          
                            if (r.Left.Equals(rule.Ptr))
                            {
                                Rule tmp = r.Copy();
                                //为新加入状态集的文法添加预测符
                                if (rule.ptr + 1 > rule.Right.Length - 1)
                                {
                                    foreach (char c in rule.ahead)
                                    {
                                        tmp.ahead.Add(c);
                                    }
                                }
                                else
                                {
                                    //if (nonterminals.Contains(rule.Right[rule.ptr + 1].ToString()))
                                    //{
                                        //非终结符后面还是非终结符的处理代码
                                    //}
                                    //else
                                    //{
                                        tmp.ahead.Add(rule.Right[rule.ptr + 1]);
                                    //}
                                    
                                }
                                //记录新加入的文法，当前状态集如果产生变化则记录它（tag变量）
                                bool newadd = true;
                                foreach (Rule newrule in newAdd)
                                {
                                    if (tmp.id == newrule.id)
                                    {
                                        foreach(char c in tmp.ahead)
                                        {
                                            if(!newrule.ahead.Contains(c))
                                            {
                                                newrule.ahead.Add(c);
                                                tag = true;
                                            }
                                        }                                      
                                        newadd = false;
                                    }
                                }
                                if (newadd)
                                {
                                    newAdd.Add(tmp);
                                    tag = true;
                                }
                            }
                        }
                    }
                }
                //将新添加的文法加入到当前状态集合中
                foreach (Rule newrule in newAdd)
                {
                    if(!ruleSet.list.Contains(newrule))
                    {
                        ruleSet.Add(newrule);
                        tag = true;
                    }               
                }
            } 
        }

        /// <summary>
        /// 通过当前状态获得下一个状态，返回一个字典，Key为状态转换条件，Value为转换后的状态
        /// </summary>
        /// <param name="ruleSet"></param>
        /// <returns></returns>
        private Dictionary<String, RuleSet> GetNextSets(RuleSet ruleSet)
        {
            Dictionary<String, RuleSet> dict = new Dictionary<String, RuleSet>();
           
            foreach(Rule rule in ruleSet.list)
            {
                RuleSet set;
                if (rule.Ptr == null)
                {
                    continue;
                }
                if(!dict.TryGetValue(rule.Ptr, out set))
                {
                    set = new RuleSet();
                    dict[rule.Ptr] = set;
                }
                Rule newRule = rule.Clone();
                newRule.ptr++;
                dict[rule.Ptr].Add(newRule);
            }

            foreach(RuleSet set in dict.Values)
            {
                foreach(RuleSet existState in state)
                {
                    if(existState.Equals(set))
                    {
                        set.id = existState.id;
                    }
                }
            }
            return dict;
        }

        /// <summary>
        /// 根据导出状态函数返回的列表修改预测表
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="thisSet"></param>
        private void AddTable(Dictionary<String, RuleSet> dict, RuleSet thisSet)
        {
            int id = thisSet.id;
            Dictionary<String, String> value;
            if(!table.TryGetValue(id, out value))
            {
                value = new Dictionary<String, String>();
                table[id] = value;
            }
            foreach(String key in dict.Keys)
            {
                String action;
                if(nonterminals.Contains(key))
                {
                    action = "g" + dict[key].id;
                }
                else
                {
                    action = "s" + dict[key].id;
                }
                value[key] = action;
            }
                
            foreach(Rule rule in thisSet.list)
            {
                if(rule.IsReduce())
                {
                    foreach(char c in rule.ahead)
                    {
                        String action = "r" + rule.id;
                        if(rule.id == 0)
                        {
                            action = "Accept";
                        }
                        value[c.ToString()] = action;
                    }
                }
            }
        }

        /// <summary>
        /// 计算LR1预测表
        /// </summary>
        public void GetTable()
        {
            Queue<RuleSet> queue = new Queue<RuleSet>();
            Rule start = rules[0];
            RuleSet ruleSet = new RuleSet();
            ruleSet.Add(start);
            queue.Enqueue(ruleSet);
            while (queue.Count != 0)
            {
                RuleSet tmp = queue.Dequeue();
                FillSet(tmp);
                Dictionary<String, RuleSet> dict = GetNextSets(tmp);
                AddTable(dict, tmp); 
                foreach(RuleSet set in dict.Values)
                {
                    bool exist = false;
                    foreach(RuleSet existSet in state)
                    {
                        if (set.Equals(existSet))
                        {
                            exist = true;
                        }
                    }
                    if (!exist)
                    {
                        queue.Enqueue(set);
                        state.Add(set);
                    }
                }                 
            }
        }

        /// <summary>
        /// 打印预测表
        /// </summary>
        public void PrintTable()
        {
            foreach(int id in table.Keys)
            {
                Console.Write("" + id + "\t");
                foreach(String v in table[id].Keys)
                {
                    Console.Write(v + " -> " + table[id][v] + " \t");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 测试一个字符串是否符合当前文法
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool Check(String str)
        {
            str += "#";
            Stack<int> stack = new Stack<int>();
            stack.Push(0);
            while(true)
            {
                if(str.Length == 0)
                {
                    return false;
                }
                String action;
                try
                {
                    action = table[stack.Peek()][str[0].ToString()];
                }
                catch(Exception e)
                {
                    return false;
                }
                
                if(action.Equals("Accept"))
                {
                    return true;
                }
                switch (action[0])
                {
                    case 's':
                        stack.Push(int.Parse(action[1].ToString()));
                        str = str.Remove(0, 1);
                        break;
                    case 'g':
                        stack.Push(int.Parse(action[1].ToString()));
                        str = str.Remove(0, 1);
                        break;
                    case 'r':
                        Rule rule = GetRuleByID(int.Parse(action[1].ToString()));
                        for (int j = 0; j < rule.Right.Length; j++ )
                        {
                            stack.Pop();
                        }
                        str = rule.Left + str;
                        break;
                }
            }
        }
    }
}
