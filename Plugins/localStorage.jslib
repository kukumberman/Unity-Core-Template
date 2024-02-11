mergeInto(LibraryManager.library, {
  LocalStorageGetItem: function (key) {
    var result = null
    try {
      result = window.localStorage.getItem(UTF8ToString(key));
    }
    catch (e) {
      console.warn(".jslib LocalStorageGetItem");
      console.error(e);
    }

    var str = result !== null ? result : "";

    var bufferSize = lengthBytesUTF8(str) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(str, buffer, bufferSize);
    return buffer;
  },

  LocalStorageSetItem: function (key, value) {
    try {
      window.localStorage.setItem(UTF8ToString(key), UTF8ToString(value));
    }
    catch (e) {
      console.warn(".jslib LocalStorageSetItem");
      console.error(e);
    }
  },

  LocalStorageRemoveItem: function(key) {
    try {
      window.localStorage.removeItem(UTF8ToString(key));
    }
    catch (e) {
      console.warn(".jslib LocalStorageRemoveItem");
      console.error(e);
    }
  },
});
