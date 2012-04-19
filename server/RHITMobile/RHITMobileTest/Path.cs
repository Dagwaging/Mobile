using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RHITMobileTest {
    public abstract class Path {
        public abstract void RunTests(string pathSoFar);
        public abstract IEnumerable<Path> GetPaths();
        public abstract string GetPath();
    }

    public class Branch : Path, ICollection<Path> {
        private List<Path> _paths = new List<Path>();
        private string _path;
        private Action<string> _failureTest;
        private bool _testEnd;
        private bool _testGarbage;
        private bool _testInt;
        private bool _testFloat;

        public Branch(string path, Action<string> failureTest, bool testEnd, bool testGarbage, bool testInt, bool testFloat) {
            _path = path;
            _failureTest = failureTest;
            _testEnd = testEnd;
            _testGarbage = testGarbage;
            _testInt = testInt;
            _testFloat = testFloat;
        }

        public void Add(Path item) {
            _paths.Add(item);
        }

        public void Clear() {
            throw new InvalidOperationException();
        }

        public bool Contains(Path item) {
            return _paths.Contains(item);
        }

        public void CopyTo(Path[] array, int arrayIndex) {
            throw new InvalidOperationException();
        }

        public int Count {
            get { return _paths.Count; }
        }

        public bool IsReadOnly {
            get { return true; }
        }

        public bool Remove(Path item) {
            throw new InvalidOperationException();
        }

        public IEnumerator<Path> GetEnumerator() {
            throw new InvalidOperationException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }

        public override void RunTests(string pathSoFar) {
            if (_testEnd) {
                _failureTest(pathSoFar);
            }
            if (_testGarbage) {
                _failureTest(pathSoFar + "/garbage");
            }
            if (_testInt) {
                _failureTest(pathSoFar + "/156");
                _failureTest(pathSoFar + "/-4");
            }
            if (_testFloat) {
                _failureTest(pathSoFar + "/156.67");
                _failureTest(pathSoFar + "/-0.67");
            }
        }

        public override IEnumerable<Path> GetPaths() {
            return _paths;
        }

        public override string GetPath() {
            return "/" + _path;
        }
    }

    public class EndOfPath : Path {
        private Action<string> _testToRun;

        public EndOfPath(Action<string> testToRun) {
            _testToRun = testToRun;
        }

        public override void RunTests(string pathSoFar) {
            _testToRun(pathSoFar);
        }

        public override IEnumerable<Path> GetPaths() {
            yield break;
        }

        public override string GetPath() {
            return "";
        }
    }
}
