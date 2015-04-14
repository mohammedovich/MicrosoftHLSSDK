/*********************************************************************************************************************
Microsft HLS SDK for Windows

Copyright (c) Microsoft Corporation

All rights reserved.

MIT License

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation
files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy,
modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software
is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

***********************************************************************************************************************/

#pragma once



#include "pch.h"  
#include "MFStreamCommonImpl.h"  

namespace Microsoft {
  namespace HLSClient {
    namespace Private {

      //forward declare
      class CHLSMediaSource;


      class CMFAudioStream : public CMFStreamCommonImpl
      {
      public:
        ///<summary>Constructor</summary>    
        ///<param name='pSource'>The parent media source</param>
        ///<param name='pplaylist'>The current playlist</param>
        CMFAudioStream(CHLSMediaSource *pSource, Playlist *pplaylist, DWORD CommonWorkQueue);
        ///<summary>See IMFAsyncCallback in MSDN</summary>
        IFACEMETHOD(Invoke)(IMFAsyncResult *pResult);
        ///<summary>Switch completed event</summary>
        void RaiseBitrateSwitched(unsigned int From, unsigned int To) override;
        ///<summary>Switch the stream to an alternate rendition</summary>
        void SwitchRendition();

        void SwitchBitrate();
        ///<summary>Construct stream descriptor</summary>
        HRESULT ConstructStreamDescriptor(unsigned int id);
        ///<summary>Destructor</summary>
        ~CMFAudioStream()
        {

        }


      };
    }
  }
} 

