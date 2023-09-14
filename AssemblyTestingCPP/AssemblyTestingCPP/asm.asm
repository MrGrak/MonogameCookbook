.code
Fn proc
	push rbx				;store

	mov rax, 123			;64 bit gp registers
	mov rbx, 123
	mov rcx, 123
	mov rdx, 123

	;rsi					;fn param register
	;rdi					;fn param register
	;rdx					;fn param register
	;rcx					;fn param register

	mov r8, 123				;fn param register
	mov r9, 123				;fn param register
	mov r10, 123
	mov r11, 123

	mov r12, 123
	mov r13, 123
	mov r14, 123
	mov r15, 123

	xorps xmm0, xmm0		;128 bit registers
	xorps xmm1, xmm1
	xorps xmm2, xmm2
	xorps xmm3, xmm3
	xorps xmm4, xmm4
	xorps xmm5, xmm5
	xorps xmm6, xmm6
	xorps xmm7, xmm7
	xorps xmm8, xmm8
	xorps xmm9, xmm9
	xorps xmm10, xmm10
	xorps xmm11, xmm11
	xorps xmm12, xmm12
	xorps xmm13, xmm13
	xorps xmm14, xmm14
	xorps xmm15, xmm15

	vmovdqa ymm0, ymm0		;256 bit registers
	vmovdqa ymm1, ymm1
	vmovdqa ymm2, ymm2
	vmovdqa ymm3, ymm3
	vmovdqa ymm4, ymm4
	vmovdqa ymm5, ymm5
	vmovdqa ymm6, ymm6
	vmovdqa ymm7, ymm7
	vmovdqa ymm8, ymm8
	vmovdqa ymm9, ymm9
	vmovdqa ymm10, ymm10
	vmovdqa ymm11, ymm11
	vmovdqa ymm12, ymm12
	vmovdqa ymm13, ymm13
	vmovdqa ymm14, ymm14
	vmovdqa ymm15, ymm15

	pop rbx					;restore
	ret
Fn endp
end