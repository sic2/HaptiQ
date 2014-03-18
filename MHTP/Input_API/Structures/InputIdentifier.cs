﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;

namespace Input_API
{
    /// <summary>
    /// This class defined an InputIdentifier.
    /// An input identifier is the abstract representation of the physical hardware used
    /// to interact with the screen.
    /// For example, for a Surface Table a ByteTag is such physical hardware.
    /// For most touch screen devices a finger is used.
    /// This class also supports Glyphs (@see Gratf project). 
    /// The reason why Glyphs are used is that there are platform independent and have resulted
    /// to be much more efficient and well performant than ByteTags.
    /// </summary>
    public class InputIdentifier
    {
        /// <summary>
        /// Unique identifier of the touch. 
        /// This is generally generated by the input device (i.e. Microsoft Table), but 
        /// it is not a strict requirement.
        /// </summary>
        private UInt64 _id;

        /// <summary>
        /// Some Input Identifier could have equivalent ids.
        /// For example, the glyph identifier ID is a number describing the actual glyph itself.
        /// However, a glyph can have other 3 representations based on the orientation.
        /// </summary>
        private List<UInt64> _equivalentIDs;

        /// <summary>
        /// Define the type of input identifier used to get input.
        /// </summary>
        public enum TYPE { error, tag, finger, blob, glyph };

        /// <summary>
        /// type indicated whether the touch is received from a finger, tag, etc..
        /// Default values are:
        /// - Tag: 0
        /// - Finger: 1
        /// - Blob: 2
        /// - Glyph: 3
        /// Additional types can be easily added. 
        /// </summary>
        private TYPE _type;

        /// <summary>
        /// Side size dimension of this input identifier.
        /// This is set to -1 if the input identifier does not require a dimension
        /// </summary>
        private int _dim;

        /// <summary>
        /// Constructor for input identifier.
        /// Automatically generates equivalent IDs if the TYPE 
        /// of this identifier is glyph.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dim"></param>
        /// <param name="id"></param>
        public InputIdentifier(TYPE type, int dim, UInt64 id)
        {
            _id = id;
            _type = type;
            _dim = dim;

            // Glyphs have equivalent types
            if (type == TYPE.glyph)
            {
                generateEquivalentIDs();
            }
            else
            {
                _equivalentIDs = new List<UInt64>();
            }
        }

        /// <summary>
        /// Copies the content of a SerializableInputIdentifier to an instance of InputIdentifier
        /// </summary>
        /// <param name="serializableInputIdentifier"></param>
        public InputIdentifier(SerializableInputIdentifier serializableInputIdentifier)
        {
            if (serializableInputIdentifier != null)
            {
                _id = serializableInputIdentifier.id;
                _type = serializableInputIdentifier.type;
                _dim = serializableInputIdentifier.dim;
                if (serializableInputIdentifier.equivalentIDs == null)
                    _equivalentIDs = new List<UInt64>();
                else
                    _equivalentIDs = serializableInputIdentifier.equivalentIDs;
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public SerializableInputIdentifier getSerializableInputIdentifier()
        {
            SerializableInputIdentifier retval = new SerializableInputIdentifier();
            retval.id = _id;
            retval.type = _type;
            retval.dim = _dim;
            if (_equivalentIDs == null)
                retval.equivalentIDs = new List<UInt64>();
            else
                retval.equivalentIDs = _equivalentIDs;
            return retval;
        }

        /// <summary>
        /// Return the ID of this input identifier.
        /// Note that some input identifier can have equivalent IDs which
        /// can be accessed using getEquivalentIDs()
        /// </summary>
        /// <returns></returns>
        public UInt64 getID()
        {
            return _id;
        }

        /// <summary>
        /// Return a list of equivalent IDs. This list contains also
        /// the ID returned by getID()
        /// Return null if there are not equivalent IDs
        /// </summary>
        /// <returns></returns>
        public List<UInt64> getEquivalentIDs()
        {
            return _equivalentIDs;
        }

        /// <summary>
        /// Return the TYPE of this input identifier
        /// </summary>
        /// <returns></returns>
        public TYPE getType()
        {
            return _type;
        }

        /// <summary>
        /// Returns the side dimension of this input identifier.
        ///  Return -1 if InputIdentifier does not require a dimension
        /// </summary>
        /// <returns></returns>
        public int getDim()
        {
            return _dim;
        }

        /// <summary>
        /// Convert a string to a type
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static TYPE stringToType(String str)
        {
            switch (str.ToLower().Trim())
            {
                case "finger":
                    return TYPE.finger;
                case "blob":
                    return TYPE.blob;
                case "tag":
                    return TYPE.tag;
                case "glyph":
                    return TYPE.glyph;
                default:
                    return TYPE.error;
            }
        }

        /// <summary>
        /// Convert a 64bits Integer to a 2D byte array with side size dim
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dim"></param>
        /// <returns></returns>
        public static byte[,] intToBinaryArray(UInt64 value, int dim)
        {
            byte[,] retval = new byte[dim, dim];

            if (value == 0) return retval;

            UInt64 current = value;

            int i = dim - 1;
            int j = dim - 1;
            while (current != 0)
            {
                if (current % 2 != 0)
                {
                    retval[j, i] = 1;
                    current--;
                }
                else
                {
                    retval[j, i] = 0;
                }
                i--;
                if (i == -1)
                {
                    j--;
                    i = dim - 1;
                }
                current = current / 2;
            }

            return retval;
        }

        /// <summary>
        /// Convert a 2D byte array to a 64bits Integer
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static UInt64 binaryArrayToInt(byte[,] data)
        {
            UInt64 retval = 0UL;
            int size = (int)Math.Sqrt(data.Length);
            int power = 0;
            for (int i = size - 1; i >= 0; i--)
            {
                for (int j = size - 1; j >= 0; j--)
                {
                    retval += data[i, j] * (UInt64)Math.Pow(2, power);
                    power++;
                }
            }
            return retval;
        }

        /// <summary>
        /// Return a list of byte[,] equivalent to the given array data, but 
        /// rotated at 90, 180 and 270 degrees
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private List<byte[,]> getEquivalentArrays(byte[,] data)
        {
            int dim = (int)Math.Sqrt(data.Length);
            int max_index = dim - 1;
            byte[,] M1 = new byte[dim, dim];
            byte[,] M2 = new byte[dim, dim];
            byte[,] M3 = new byte[dim, dim];
            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    var v = data[i, j];
                    // 90 deg
                    M1[max_index - j, i] = v;
                    // 180 deg
                    M2[max_index - i, max_index - j] = v;
                    // 270 deg
                    M3[j, max_index - i] = v;                
                }
            }
            List<byte[,]> retval = new List<byte[,]>();
            retval.Add(M1);
            retval.Add(M2);
            retval.Add(M3);
            return retval;
        }

        /// <summary>
        /// Generates a set of equivalent ids for this input identifier.
        /// Equivalent ids can be accessed via getEquivalentIDs() 
        /// </summary>
        private void generateEquivalentIDs()
        {
            if (_equivalentIDs == null && _dim != -1)
            {
                byte[,] data = intToBinaryArray(_id, _dim);
                List<byte[,]> equivalentArrays = getEquivalentArrays(data);
                _equivalentIDs = new List<UInt64>();
                foreach (byte[,] array in equivalentArrays)
                {
                    _equivalentIDs.Add(binaryArrayToInt(array));
                }
                _equivalentIDs.Add(_id);
                _equivalentIDs.Sort();
            }
        }

        /// <summary>
        /// Overriding Equals is necessary to define when two input identifiers are equal 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false
            if (obj == null) return false;

            // If parameter cannot be cast to Point return false
            InputIdentifier p = obj as InputIdentifier;
            if ((System.Object)p == null) return false;

            // Compare equivalent ids
            bool equivalentIDs = true;
            if ( p._type == this._type && p._equivalentIDs != null)
            {
                List<UInt64> eq = p.getEquivalentIDs();
                foreach (UInt64 id in eq)
                {
                    if (!_equivalentIDs.Contains(id))
                    {
                        equivalentIDs = false;
                        break;
                    }
                }
            }
            else
            {
                equivalentIDs = false;
            }
            // Return true if the fields match
            return ((p._id == this._id || equivalentIDs) && p._type == this._type);
        }

        // @see: http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                
                hash = hash * 23 + this._type.GetHashCode();

                if (_equivalentIDs != null)
                {
                    foreach (UInt64 id in _equivalentIDs)
                    {
                        hash = hash * 23 + id.GetHashCode();
                    }
                }
                else
                {
                    hash = hash * 23 + this._id.GetHashCode();
                }
                return hash;
            }
        }
    }
}
