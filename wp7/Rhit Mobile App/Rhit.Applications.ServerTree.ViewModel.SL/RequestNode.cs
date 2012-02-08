using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Rhit.Applications.Model.Services.Requests;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Rhit.Applications.ViewModel {
    public class RequestNode {

        public RequestNode(Type type, RequestPart request) {
            Type = type;
            Name = type.Name;
            Request = request;
            Children = new ObservableCollection<RequestNode>();
            CreatePropertyTree();
        }

        public void CreatePropertyTree() {
            foreach(MemberInfo mi in Type.GetMembers()) {
                Type type;
                RequestPart request;
                if(mi.MemberType == MemberTypes.Method) {
                    type = (mi as MethodInfo).ReturnType;
                    try {
                        request = (RequestPart) Type.InvokeMember(mi.Name, BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, Request, new object[] { 0 });
                    } catch { continue; }
                } else if(mi.MemberType == MemberTypes.Property) {
                    type = (mi as PropertyInfo).PropertyType;
                    request = (mi as PropertyInfo).GetValue(Request, null) as RequestPart;
                } else continue;
                if(!type.IsSubclassOf(typeof(RequestPart))) continue;
                RequestNode node = new RequestNode(type, request) {
                    Name = mi.Name,
                };
                Children.Add(node);
            }
        }

        public string Name { get; set; }

        public Type Type { get; set; }

        public RequestPart Request { get; set; }

        public string Response { get; set; }

        public bool ReEvaluate(int id) {
            foreach(MemberInfo mi in Type.GetMember("Id")) {
                if(mi.MemberType == MemberTypes.Property) {
                    int tmp = (int) (mi as PropertyInfo).GetValue(Request, null);
                    if(id == tmp) continue;
                    (mi as PropertyInfo).SetValue(Request, id, null);
                    return true;
                }
            }
            return false;
        }

        public ObservableCollection<RequestNode> Children { get; set; }
    }
}
