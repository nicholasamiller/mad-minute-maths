'use client'

import React, { useState, useEffect } from 'react'

function generateQuestion(): string {
  const max = 12
  const operations = ['+', '-', '*', '/']
  const operation = operations[Math.floor(Math.random() * operations.length)]
  let num1: number
  let num2: number

  switch (operation) {
    case '+':
    case '-':
      // Decide whether to use 3 or 4 terms
      const termCount = Math.floor(Math.random() * 2) + 3 // 3 or 4
      const terms: number[] = []
      const ops: string[] = []

      // Generate first term
      terms.push(Math.floor(Math.random() * max) + 1)

      // Generate subsequent terms and operators
      for (let i = 1; i < termCount; i++) {
        const operator = Math.random() < 0.5 ? '+' : '-'
        ops.push(operator)
        terms.push(Math.floor(Math.random() * max) + 1)
      }

      // Calculate the result to ensure it's >= 0
      let result = terms[0]
      for (let i = 0; i < ops.length; i++) {
        if (ops[i] === '+') {
          result += terms[i + 1]
        } else {
          result -= terms[i + 1]
        }
      }

      // If result is negative, regenerate the question
      if (result < 0) {
        return generateQuestion()
      }

      // Build the equation string
      let equation = `${terms[0]}`
      for (let i = 0; i < ops.length; i++) {
        equation += ` ${ops[i]} ${terms[i + 1]}`
      }
      equation += ' = ____'
      return equation

    case '*':
      num1 = Math.floor(Math.random() * max) + 1
      num2 = Math.floor(Math.random() * max) + 1
      return `${num1} × ${num2} = ____`
    case '/':
      num2 = Math.floor(Math.random() * max) + 1
      num1 = num2 * (Math.floor(Math.random() * max) + 1)
      return `${num1} ÷ ${num2} = ____`
    default:
      return ''
  }
}

function generateQuestions(count: number): string[] {
  return Array(count).fill(null).map(() => generateQuestion())
}

export default function Home() {
  const [questions, setQuestions] = useState<string[]>([])

  useEffect(() => {
    setQuestions(generateQuestions(20))
  }, [])

  return (
    <div className="container mx-auto p-4 max-w-4xl">
      <h1 className="text-3xl font-bold mb-4 text-center">Millie&apos;s Mad Minute</h1>
      <p className="mb-4 text-center">Complete as many problems as you can in 1 minute. Go, Millie, go!</p>
      <div className="grid grid-cols-2 gap-4">
        {questions.map((question, index) => (
          <div key={index} className="border-2 border-gray-300 rounded p-4 text-center text-xl bg-white shadow">
            {question}
          </div>
        ))}
      </div>
      <div className="mt-8">
        <p>Date: _______________</p>
        <p>Score: ___ / 20</p>
      </div>
    </div>
  )
}
